/*
 *
    COPYRIGHT LICENSE: This information contains sample code provided in source code form. You may copy, modify, and distribute
    these sample programs in any form without payment to IBM® for the purposes of developing, using, marketing or distributing
    application programs conforming to the application programming interface for the operating platform for which the sample code is written.
    Notwithstanding anything to the contrary, IBM PROVIDES THE SAMPLE SOURCE CODE ON AN "AS IS" BASIS AND IBM DISCLAIMS ALL WARRANTIES,
    EXPRESS OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, ANY IMPLIED WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
    FITNESS FOR A PARTICULAR PURPOSE, TITLE, AND ANY WARRANTY OR CONDITION OF NON-INFRINGEMENT. IBM SHALL NOT BE LIABLE FOR ANY DIRECT,
    INDIRECT, INCIDENTAL, SPECIAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR OPERATION OF THE SAMPLE SOURCE CODE.
    IBM HAS NO OBLIGATION TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS OR MODIFICATIONS TO THE SAMPLE SOURCE CODE.

 */

using IBM.Worklight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InvokingAdapterProceduresWin8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage _this;
        public JToken mainPageJSON = null;


        public MainPage()
        {
            this.InitializeComponent();
            ConnectButton.Click += new RoutedEventHandler(ConnectButton_Click);
            InvokeButton.Click += new RoutedEventHandler(InvokeButton_Click);
            _this = this; 
        }

        void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ReceivedTextBlock.Text = "\nConnecting...\n";
            WLClient client = WLClient.getInstance();
            client.connect(new MyConnectResponseListener(this));
        }

        void InvokeButton_Click(object sender, RoutedEventArgs e)
        {
            ReceivedTextBlock.Text = "\nInvoking Procedure...\n";
            WLProcedureInvocationData invocationData = new WLProcedureInvocationData("RSSReader", "getFeeds");
            invocationData.setParameters(new Object[] { });
            String myContextObject = "InvokingAdapterProceduresWin8";
            WLRequestOptions options = new WLRequestOptions();
            options.setInvocationContext(myContextObject);
            WLClient.getInstance().invokeProcedure(invocationData, new MyInvokeListener(this), options);
        }

         

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public class MyConnectResponseListener : WLResponseListener
        {
            InvokingAdapterProceduresWin8.MainPage myMainPage;
            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            public MyConnectResponseListener(InvokingAdapterProceduresWin8.MainPage page)
            {
                myMainPage = page;
            }

            public async void onSuccess(WLResponse response)
            {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               myMainPage.ReceivedTextBlock.Text="\nConnected Successfuly";
                 
            });
               

            
            }

            public async void onFailure(WLFailResponse response)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    myMainPage.ReceivedTextBlock.Text="\nFailed ";
                });
            }

        }

        public class MyInvokeListener : WLResponseListener
        {
            InvokingAdapterProceduresWin8.MainPage myMainPage;
            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            public MyInvokeListener(InvokingAdapterProceduresWin8.MainPage page)
            {
                myMainPage = page;
            }

            public void onSuccess(WLResponse response)
            {
                WLProcedureInvocationResult invocationResponse = ((WLProcedureInvocationResult)response);
                JObject items;
                try
                {
                  
              CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    StringBuilder result = new StringBuilder();
                    JObject feed = (JObject)response.getResponseJSON().GetValue("rss");
                    //if the return value is from the invoke procedure - we wil have a element called rss
                    JToken itemList = null;
                    if (feed != null)
                    {
                        //parse the result for pretty print
                        itemList = ((JObject)feed.GetValue("channel")).GetValue("item");
                        MainPage._this.mainPageJSON = itemList;
                        for (int i = 0; i < itemList.Count(); i++)
                        {
                            if (String.Compare(((JObject)itemList[i]).GetValue("description").ToString(), "")>0)
                            {
                               result.AppendLine("\n" + ((JObject)itemList[i]).GetValue("description").ToString());
                               result.AppendLine("\n------------------------------------------------------------------------------------------------------------------------------------------");
                            }
                        }
                        MainPage._this.ReceivedTextBlock.Text = "\n\n" + result;
                    }

                });
                   
                }
                catch (JsonReaderException e)
                {
                    Debug.WriteLine("JSONException : " + e.Message);
                }
            }

            public async void onFailure(WLFailResponse response)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                   myMainPage.ReceivedTextBlock.Text = "\nResponse failed: " + response.getErrorMessage();
                });
            }
        }


    }
}
