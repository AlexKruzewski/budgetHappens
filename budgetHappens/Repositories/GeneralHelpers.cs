using budgetHappens.Models;
using budgetHappens.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace budgetHappens.Repositories
{
    public static class GeneralHelpers
    {
        public static List<string> GetNames<TEnum>() where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException(String.Format("Type '{0}' is not an enum", type.Name));

            return (
              from field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
              where field.IsLiteral
              select field.Name)
            .ToList<string>();
        }

        /// <summary>
        /// Gets the date of the given day in the week closest to before the currenct date
        /// </summary>
        /// <param name="dt">Todays Date</param>
        /// <param name="startOfWeek">The day of the week</param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Returns the list of currencies available
        /// </summary>
        /// <returns>Returns a list of currencies</returns>
        public static List<string> GetCurrencies()
        {
            List<string> currencies = new List<string>();
            currencies.Add("£");
            currencies.Add("$");
            currencies.Add("€");
            return currencies;
        }

        public static void GetItemsRecursive<T>(DependencyObject parents, ref List<T> objectList) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parents);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parents, i);
                if (child is T)
                {
                    objectList.Add(child as T);
                }
                GetItemsRecursive<T>(child, ref objectList);
            }

            return;
        }

        /// <summary>
        /// Validates the given data type
        /// </summary>
        /// <param name="value">The values to be validated</param>
        /// <param name="dataType">The datatype to validate it agasint.</param>
        /// <returns></returns>
        public static bool ValidateValue(object value, DataType dataType)
        {
            bool validates = true;

            switch (dataType)
            {
                case DataType.Int:
                    int tempInt;
                    validates = int.TryParse(value.ToString(), out tempInt);
                    break;
                case DataType.Decimal:
                    decimal tempDecimal;
                    validates = decimal.TryParse(value.ToString(), out tempDecimal);
                    if (tempDecimal < 0)
                        validates = false;
                    break;
                case DataType.Date:
                    DateTime tempDate;
                    validates = DateTime.TryParse(value.ToString(), out tempDate);
                    break;
                case DataType.String:
                    validates = !String.IsNullOrEmpty(value.ToString());
                    break;
            }

            return validates;
        }

        /// <summary>
        /// Updates the live tile based on the given budget
        /// </summary>
        /// <param name="budget">Budget to set the live tile data</param>
        public static void UpdateLiveTile(BudgetModel budget)
        {
            ShellTile oTile = ShellTile.ActiveTiles.First();

            if (oTile != null)
            {
                FlipTileData oFliptile = new FlipTileData();
                if (budget != null)
                {
                    oFliptile.Title = "Budget Happens";
                    oFliptile.BackgroundImage = new Uri("Assets/Tiles/logo-med.png", UriKind.Relative);
                    oFliptile.BackTitle = "Budget Happens";
                    oFliptile.BackContent = "";

                    string[] stringArray = new string[3];
                    stringArray[0] = string.Format("{0}", budget.Name);
                    stringArray[1] = string.Format("{0}{1}", budget.Currency, budget.CurrentPeriod.CurrentAmount.ToString("0.00"));
                    stringArray[2] = string.Format("{0} days left", budget.CurrentPeriod.DaysLeft.ToString("0"));

                    RenderText(stringArray, 336, 336, 64, "BackBackgroundImage");
                    oFliptile.BackBackgroundImage = new Uri(@"isostore:/Shared/ShellContent/BackBackgroundImage.jpg", UriKind.Absolute);
                }
                else
                    oFliptile = ClearFlipTileBack();

                oTile.Update(oFliptile);
            }
        }

        /// <summary>
        /// Clears the data in the back of the flip tile to stop it
        /// from flipping and showing invalid data.
        /// </summary>
        /// <returns>FlipTileData</returns>
        private static FlipTileData ClearFlipTileBack()
        {
            var clearTileBackXml = new StringBuilder();

            clearTileBackXml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            clearTileBackXml.Append("<wp:notification xmlns:wp=\"WPNotification\" version=\"2.0\">");
            clearTileBackXml.Append("<wp:tile template=\"FlipTile\">");
            clearTileBackXml.Append("<wp:WideBackBackgroundImage action=\"Clear\"></wp:WideBackBackgroundImage>");
            clearTileBackXml.Append("<wp:WideBackContent action=\"Clear\"></wp:WideBackContent>");
            clearTileBackXml.Append("<wp:BackBackgroundImage action=\"Clear\"></wp:BackBackgroundImage>");
            clearTileBackXml.Append("<wp:BackContent action=\"Clear\"></wp:BackContent>");
            clearTileBackXml.Append("<wp:BackTitle action=\"Clear\"></wp:BackTitle>");
            clearTileBackXml.Append("</wp:tile>");
            clearTileBackXml.Append("</wp:notification>");
           
            return new FlipTileData(clearTileBackXml.ToString());

        }

        /// <summary>
        /// Creates a image of the live tile background from the given text
        /// </summary>
        /// <param name="text">String array of the texts to add to the back of a live tile</param>
        /// <param name="width">Width of live tile</param>
        /// <param name="height">Height of live tile</param>
        /// <param name="fontsize">Font size of the text</param>
        /// <param name="imagename">Image name to be saved locally</param>
        private static void RenderText(string[] text, int width, int height, int fontsize, string imagename)
        {
            WriteableBitmap b = new WriteableBitmap(width, height);

            var canvas = new Grid();
            canvas.Width = b.PixelWidth;
            canvas.Height = b.PixelHeight;

            var background = new Canvas();
            background.Height = b.PixelHeight;
            background.Width = b.PixelWidth;

            //Created background color as Accent color
            SolidColorBrush backColor = Convert(AppResources.SecondaryColor);
            background.Background = backColor;

            var budgetName = new TextBlock();
            budgetName.Text = text[0];
            budgetName.Margin = new Thickness(10.00, 5, 0, 0);
            budgetName.TextAlignment = TextAlignment.Left;
            budgetName.HorizontalAlignment = HorizontalAlignment.Center;
            budgetName.VerticalAlignment = VerticalAlignment.Center;
            budgetName.Width = b.PixelWidth - budgetName.Margin.Left;
            budgetName.TextWrapping = TextWrapping.Wrap;
            budgetName.Foreground = new SolidColorBrush(Colors.White); //color of the text on the Tile
            budgetName.FontSize = 48;

            canvas.Children.Add(budgetName);

            var currentAmount = new TextBlock();
            currentAmount.Text = text[1];
            currentAmount.Margin = new Thickness(75.00, 100.00, 0, 0);
            currentAmount.TextAlignment = TextAlignment.Left;
            currentAmount.HorizontalAlignment = HorizontalAlignment.Center;
            currentAmount.VerticalAlignment = VerticalAlignment.Center;
            currentAmount.Width = b.PixelWidth - currentAmount.Margin.Left;
            currentAmount.TextWrapping = TextWrapping.Wrap;
            currentAmount.Foreground = new SolidColorBrush(Colors.White); //color of the text on the Tile
            currentAmount.FontSize = fontsize;

            canvas.Children.Add(currentAmount);

            var daysLeft = new TextBlock();
            daysLeft.Text = text[2];
            daysLeft.Margin = new Thickness(90.00, 180.00, 0, 0);
            daysLeft.TextAlignment = TextAlignment.Left;
            daysLeft.HorizontalAlignment = HorizontalAlignment.Center;
            daysLeft.VerticalAlignment = VerticalAlignment.Center;
            daysLeft.Width = b.PixelWidth - currentAmount.Margin.Left;
            daysLeft.TextWrapping = TextWrapping.Wrap;
            daysLeft.Foreground = new SolidColorBrush(Colors.White); //color of the text on the Tile
            daysLeft.FontSize = fontsize/2;

            canvas.Children.Add(daysLeft);

            b.Render(background, null);
            b.Render(canvas, null);
            b.Invalidate(); //Draw bitmap

            //Save bitmap as jpeg file in Isolated Storage
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + imagename + ".jpg", System.IO.FileMode.Create, isf))
                {
                    b.SaveJpeg(imageStream, b.PixelWidth, b.PixelHeight, 0, 100);
                }
            }
        }

        /// <summary>
        /// Converts a hexadecimal string value into a Brush.
        /// </summary>
        private static SolidColorBrush Convert(string value)
        {
            byte alpha;
            byte pos = 0;

            string hex = value.ToString().Replace("#", "");

            if (hex.Length == 8)
            {
                alpha = System.Convert.ToByte(hex.Substring(pos, 2), 16);
                pos = 2;
            }
            else
            {
                alpha = System.Convert.ToByte("ff", 16);
            }

            byte red = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte green = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte blue = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            return new SolidColorBrush(Color.FromArgb(alpha, red, green, blue));
        }
    }
}
