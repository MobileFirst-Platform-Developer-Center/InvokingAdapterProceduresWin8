/**
* Copyright 2015 IBM Corp.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
 
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using IBM.Worklight;
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;
using Windows.UI;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InvokingAdapterProceduresWin8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WLClient wlClient = null;
        public static MainPage _this;

        public MainPage()
        {
            this.InitializeComponent();
            _this = this;
        }

        private void ConnectServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     MainPage._this.Console.Text = "\n\nConnecting to server";

                 });


                wlClient = WLClient.getInstance();

                MyResponseListener mylistener = new MyResponseListener(this);
                wlClient.connect(mylistener);
            }
            catch (Exception ex)
            { Debug.WriteLine(ex.StackTrace); }
        }


        private void invokeProcedure_Click(object sender, RoutedEventArgs e)
        {
            Uri adapterPath = new Uri("/adapters/RSSReader/getFeeds");
            WLResourceRequest request = new WLResourceRequest(adapterPath, "GET");
            

            Object[] parameters = { 0 };
            MyInvokeListener listener = new MyInvokeListener(this);
            request.send(listener);

        }

        public void AddTextToConsole(String consoleText)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     MainPage._this.Console.Text = consoleText;

                 });
        }

        private void ClearConsole(object sender, DoubleTappedRoutedEventArgs e)
        {
            Console.Text = "";
        }

        public static void displayFeed(WLResponse response)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    StringBuilder result = new StringBuilder();
                    JObject feed = (JObject)response.getResponseJSON().GetValue("rss");
                    JToken itemList = null;
                    if (feed != null)
                    {
                        itemList = ((JObject)feed.GetValue("channel")).GetValue("item");
                        for (int i = 0; i < itemList.Count(); i++)
                        {
                            result.AppendLine("--" + ((JObject)itemList[i]).GetValue("description").ToString());
                        }
                    }
                    MainPage._this.Console.Text = "\n" + result;

                });

        }

    }
}
