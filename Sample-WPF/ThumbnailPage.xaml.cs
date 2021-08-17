//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/Cognitive-Vision-Windows
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namespace for ComputerVisionClient.
// -----------------------------------------------------------------------
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace VisionAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for ThumbnailPage.xaml.
    /// </summary>
    public partial class ThumbnailPage : ImageScenarioPage
    {
        public ThumbnailPage()
        {
            InitializeComponent();
            this.URLTextBox = _urlTextBox;
        }

        /// <summary>
        /// Uploads the image to Cognitive Services and generates a thumbnail.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="width">Width of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="height">Height of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="smartCropping">Boolean flag for enabling smart cropping.</param>
        /// <returns>Awaitable stream containing the image thumbnail.</returns>
        private async Task<Stream> UploadAndThumbnailImageAsync(string imageFilePath, int width, int height, bool smartCropping)
        {
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Cognitive Services Vision API Service client.
            //
            using (var client = new ComputerVisionClient(Credentials) { Endpoint = Endpoint })
            {
                Log("ComputerVisionClient is created");

                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    //
                    // Upload an image and generate a thumbnail.
                    //
                    Log("Calling ComputerVisionClient.GenerateThumbnailInStreamAsync()...");
                    return await client.GenerateThumbnailInStreamAsync(width, height, imageFileStream, smartCropping);
                }
            }

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Sends a URL to Cognitive Services and generates a thumbnail.
        /// </summary>
        /// <param name="imageUrl">The URL of the image for which to generate a thumbnail.</param>
        /// <param name="width">Width of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="height">Height of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="smartCropping">Boolean flag for enabling smart cropping.</param>
        /// <returns>Awaitable stream containing the image thumbnail.</returns>
        private async Task<Stream> ThumbnailUrlAsync(string imageUrl, int width, int height, bool smartCropping)
        {
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Cognitive Services Vision API Service client.
            //
            using (var client = new ComputerVisionClient(Credentials) { Endpoint = Endpoint })
            {
                Log("ComputerVisionClient is created");

                //
                // Generate a thumbnail for the given URL.
                //
                Log("Calling ComputerVisionClient.GenerateThumbnailAsync()...");
                Stream result;
                result = await client.GenerateThumbnailAsync(width, height, imageUrl, smartCropping);
                return result;
            }

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Perform the work for this scenario.
        /// </summary>
        /// <param name="imageUri">The URI of the image to run against the scenario.</param>
        /// <param name="upload">Upload the image to Cognitive Services if [true]; submit the Uri as a remote URL if [false].</param>
        protected override async Task DoWorkAsync(Uri imageUri, bool upload)
        {
            _status.Text = "Generating Thumbnail...";

            //
            // Get the parameters.
            //
            //int width = int.Parse(_widthTextBox.Text);
            //int height = int.Parse(_heightTextBox.Text);
            //bool useSmartCropping = _smartCroppingCheckbox.IsChecked.Value;

            //
            // Either upload an image, or supply a URL.
            //
            Stream thumbnailStream;
            if (upload)
            {
                thumbnailStream = await UploadAndThumbnailImageAsync(imageUri.LocalPath, 200, 200, true);
            }
            else
            {
                thumbnailStream = await ThumbnailUrlAsync(imageUri.AbsoluteUri, 200, 200, true);
            }
            _status.Text = "Result Generated";

            //
            // Show the generated thumbnail in the GUI.
            //
            BitmapImage thumbSource = new BitmapImage();
            thumbSource.BeginInit();
            thumbSource.CacheOption = BitmapCacheOption.None;
            thumbSource.StreamSource = thumbnailStream;
            thumbSource.EndInit();
            Log("Generated Thumbnail");
        }

        protected async Task<BitmapImage> GetThumbnail(Uri imageUri, bool upload)
        {
            Log("Generating Thumbnail");
            Stream thumbnailStream;
            if (upload)
            {
                thumbnailStream = await UploadAndThumbnailImageAsync(imageUri.LocalPath, 100, 100, true);
            }
            else
            {
                thumbnailStream = await ThumbnailUrlAsync(imageUri.AbsoluteUri, 100, 100, false);
            }

            BitmapImage thumbSource = new BitmapImage();
            thumbSource.BeginInit();
            thumbSource.CacheOption = BitmapCacheOption.None;
            thumbSource.StreamSource = thumbnailStream;
            thumbSource.EndInit();
            Log("Generated Thumbnail");

            return thumbSource;

        }

        private async Task GenerateTableRow(List<ImageData> res)
        {
            var rowGroup = _tableGroup1.RowGroups[0];
            rowGroup.Rows.Clear();


            TableRow row1 = new TableRow();
            row1.Background = Brushes.Gray;

            TableCell cell1 = new TableCell();

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add("Image url");
            paragraph.FontSize = 14;
            paragraph.FontWeight = FontWeights.Bold;
            cell1.Blocks.Add(paragraph);
            row1.Cells.Add(cell1);

            cell1 = new TableCell();

            paragraph = new Paragraph();
            paragraph.Inlines.Add("Image");
            paragraph.FontSize = 14;
            paragraph.FontWeight = FontWeights.Bold;
            cell1.Blocks.Add(paragraph);
            row1.Cells.Add(cell1);

            cell1 = new TableCell();

            paragraph = new Paragraph();
            paragraph.Inlines.Add("Tags");
            paragraph.FontSize = 14;
            paragraph.FontWeight = FontWeights.Bold;
            cell1.Blocks.Add(paragraph);
            row1.Cells.Add(cell1);

            rowGroup.Rows.Add(row1);


            foreach (var item in res)
            {
                    TableRow row = new TableRow();

                    TableCell cell = new TableCell();

                    Paragraph paragraphUrl = new Paragraph();
                    TextBlock textBlockUrl = new TextBlock();
                    textBlockUrl.Text = item.imageUrl;
                    textBlockUrl.Padding = new Thickness(5,5,5,5);
                    textBlockUrl.TextWrapping = TextWrapping.WrapWithOverflow; 
                    //Hyperlink hyperlink = new Hyperlink();
                   //hyperlink.NavigateUri = new Uri(item.imageUrl);

                    //textBlockUrl.Inlines.Add(hyperlink);
                    paragraphUrl.Inlines.Add(textBlockUrl);
                    cell.Blocks.Add(paragraphUrl);
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    Paragraph paragraphImage = new Paragraph();
                    Image image = new Image();
                    Uri imageUri = new Uri(item.imageUrl);
                    image.Source = await GetThumbnail(imageUri, false);
                    image.Width = 100;
                    image.Height = 100;
                    paragraphImage.Inlines.Add(image);
                    cell.Blocks.Add(paragraphImage);
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    Paragraph paragraphTag = new Paragraph();
                    TextBlock textBlockTag = new TextBlock();
                    textBlockTag.Margin = new Thickness(5, 5, 5, 5);
                    textBlockTag.Text = string.Join(",", item.confidenceByTag.Keys);
                    textBlockUrl.TextWrapping = TextWrapping.WrapWithOverflow;
                    paragraphTag.Inlines.Add(textBlockTag);
                    cell.Blocks.Add(paragraphTag);
                    row.Cells.Add(cell);

                    rowGroup.Rows.Add(row);
                
            }
        }

        private async void SubmitUriButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTag = URLTextBox.Text;
            List<string> tags = searchTag.Split(',').Select(x => x.Trim().ToLower()).ToList();
            List<ImageData> res = GetImages_Click(tags);
            if (res != null && res.Count > 0)
            {
                await GenerateTableRow(res);
                _imagesGrid.Visibility = Visibility.Visible;
                Log("Image Result Recieved");
            }
            else
            {
                _imagesGrid.Visibility = Visibility.Hidden;
                Log("No Images Found for the entered tag");
            }

        }

        private List<ImageData> GetImages_Click(List<string> tags)
        {
            var images = SqlHelper.GetImages(tags);

            foreach (string tag in tags)
            {
                foreach (ImageData image in images)
                {
                    if (image.confidenceByTag.ContainsKey(tag.ToLower()))
                    {
                        image.score += image.confidenceByTag[tag.ToLower()];
                    }
                }
            }

            var result = images.Where(x=> x.score > 0).OrderByDescending(i => i.score).Take(5).ToList<ImageData>();

            return result;

        }
    }
}
