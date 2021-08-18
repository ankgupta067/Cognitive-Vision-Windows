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

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namespace for ComputerVisionClient.
// -----------------------------------------------------------------------
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace VisionAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for UploadImage.xaml.
    /// </summary>
    /// <summary>
    /// Interaction logic for UploadImage.xaml.
    /// </summary>
    public partial class UploadImage : ImageScenarioPage
    {
        private TagResult analysisResult;
        private List<string> finalTags = new List<string>();
        public UploadImage()
        {
            InitializeComponent();
            this.PreviewImage = _imagePreview;
            this.URLTextBox = _urlTextBox;
            this._language.ItemsSource = RecognizeLanguage.SupportedForAnalysis;
        }

        private void DynamicCreatedCheckbox(TagResult tagList)
        {
            _imageTags.Children.Clear();
            foreach (var item in tagList.Tags)
            {
                CheckBox chk = new CheckBox();
                chk.Content = item.Name.ToString();
                chk.Tag = item;
                _imageTags.Children.Add(chk);
                chk.IsChecked = true;
            }

        }

        /// <summary>
        /// Uploads the image to Cognitive Services and performs analysis.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <returns>Awaitable image analysis result.</returns>
        private async Task<TagResult> UploadImageAndAnalyzeTagsAsync(string imageUrl)
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
                // Generate tags for the given URL.
                //
                Log("Calling ComputerVisionClient.TagImageAsync()...");
                string language = (_language.SelectedItem as RecognizeLanguage).ShortCode;
                return await client.TagImageAsync(imageUrl, language);
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
        /// <summary>
        /// Perform the work for this scenario.
        /// </summary>
        /// <param name="imageUri">The URI of the image to run against the scenario.</param>
        /// <param name="upload">Upload the image to Cognitive Services if [true]; submit the Uri as a remote URL if [false].</param>
        protected override async Task DoWorkAsync(Uri imageUri, bool upload)
        {
            _status.Text = "Analyzing...";

            analysisResult = null;
            analysisResult = await UploadImageAndAnalyzeTagsAsync(imageUri.AbsoluteUri);

            _status.Text = "Analyzing Done";

            DynamicCreatedCheckbox(analysisResult);
            _tagsGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void SubmitTagsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var tags = new List<Tag>();
            foreach (CheckBox item in _imageTags.Children)
            {
                if (item.IsChecked == true)
                {
                    tags.Add(new Tag()
                    {
                        TagValue = item.Content.ToString().ToLower(),
                        Confidence = ((ImageTag) (item.Tag)).Confidence
                    });
                }
            }

            string userDefinedTags = text1.Text;
            string[] userTagList = userDefinedTags.ToString().Split(',');
            foreach (string tag in userTagList)
            {
                if (!string.IsNullOrWhiteSpace(tag) && !finalTags.Contains(tag.Trim()))
                {
                    tags.Add(new Tag()
                    {
                        TagValue = tag.Trim().ToLower(),
                        Confidence = 1
                    });
                }


            }

            Log("Data Ready to Upload to DB");

            var result = SqlHelper.SaveImage(URLTextBox.Text, tags);
            _status.Text = result.Item1 ? "Data saved in DB" : $"Error in saving data to DB - {result.Item2}";

            Log("Operation Completed");

        }

        private void GetImages_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var tags = new List<string>(){"laptop","Mac"};
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

            var result = images.OrderByDescending(i => i.score).Take(5).ToList<ImageData>();

        }
    }
}