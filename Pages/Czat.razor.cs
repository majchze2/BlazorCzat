using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Grpc.Net.Client;
using GrpcclientCzat;
using Grpc.Core;
using Grpc.Net.Client.Web;
using Radzen.Blazor.Rendering;

namespace KlientRPC.Client.Pages
{
    public partial class Czat
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected IEnumerable<string> co;
        protected List<string> emojis = new List<string>() { "\U0001F600", "\U0001F601", "\U0001F602", "\U0001F603", "\U0001F64F", "\U0001F604", "\U0001F605", "\U0001F606", "\U0001F607", "\U0001F608", "\U0001F609", "\U0001F60F" };
        protected List<string> list;
        protected string ed = "";
        protected string ss = "";
        protected string nick = "";
        protected bool but1Dis = true;
        protected bool but2Dis = false;
        protected bool labelnick = false;
        protected string labeltext = "";
        protected bool panelUsers = true;
        protected string button0text = "-";
        protected async Task AddEmoji(string emoji)
        {
            ed = ed + emoji;
            StateHasChanged();
        }
        private List<string> getUser(string users)
        {
            string us = "";
            string users1 = users;
            List<string> list = new List<string>();
            users1 = users1.Remove(0, 1);
            int np = users1.IndexOf(";");
            while (np >= 0)
            {
                if (np > 0)
                {
                    us = users1.Substring(0, np).Trim();
                    list.Add(us);
                    users1 = users1.Remove(0, np + 1);
                }
                np = users1.IndexOf(";");
                if (np < 0)
                    list.Add(users1);
            }
            return list;
        }
        protected async override void OnInitialized()
        {
            // await base.OnInitializedAsync();
            // List<string> list = new List<string>();
            //list.Add("Zenek");
            // list.Add("Wojtek");
            //co=list
        }
        protected async System.Threading.Tasks.Task Confirm()
        {
            int n = 0;
        }
        protected async System.Threading.Tasks.Task ClientDeleteButtonClick()
        {
            var confirmResult = await DialogService.Confirm();

            if (confirmResult.HasValue && confirmResult.Value)
            {
                try
                {
                    //await ClientService.Delete(clientId);
                    int n = 0;
                }
                catch (System.Exception exception)
                {
                    NotificationService.Notify(NotificationSeverity.Error, $"Error",
                        $"Foo", duration: -1);

                }
            }
        }


        protected async System.Threading.Tasks.Task Button2Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            var channel = GrpcChannel.ForAddress("https://grpcczat.azurewebsites.net", new GrpcChannelOptions { HttpClient = httpClient });
            var client = new GrpcclientCzat.ChatService.ChatServiceClient(channel);
            try
            {

                var reply = client.sendMsgAsync(
                                new ChatMessage { From = nick, Msg = ed, Time = DateTime.Now.ToString() });
            }
            catch (Exception ex)
            {
                int n = 0;
                ss = "ERROR";
            }
            StateHasChanged();
        }

        protected async System.Threading.Tasks.Task Button3Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            but2Dis = false;
            but1Dis = true;
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        }

        protected async System.Threading.Tasks.Task Button4Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            if (nick.Trim() == "")
            {
                labelnick = true;
                labeltext = "Nie podano nick!";
                return;
            }
            else
            {
                labelnick = false;
                labeltext = "";
            }
            //var channel = GrpcChannel.ForAddress("https://grpcnetczat1.azurewebsites.net", new GrpcChannelOptions { HttpClient = httpClient });
            //var channel = GrpcChannel.ForAddress("https://localhost:7280");
            //var channel = GrpcChannel.ForAddress("https://grpcnetczat1.azurewebsites.net");
            ///var client = new ChatService.ChatServiceClient(channel);
            ///var r = client.joinAsync(new User { Id = "777", Name = "Wojtek" });
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            var channel = GrpcChannel.ForAddress("https://grpcczat.azurewebsites.net", new GrpcChannelOptions { HttpClient = httpClient });
            //var channel = GrpcChannel.ForAddress("https://localhost:7280");
            var client = new GrpcclientCzat.ChatService.ChatServiceClient(channel);
            var reply01 = await client.getAllUsersAsync(new Empty { });
            string s = reply01.Users.ToString();// ResponseAsync.Result.Users.ToString();
            if (s.Contains(nick))
            {
                var confirmResult = await DialogService.Confirm("Taki u¿ytkownik jest ju¿ w u¿yciu. Czy ma byæ dalej?");
                if (confirmResult.Value == false)
                    return;
                // labelnick = true;
                // labeltext="Taki Nick ju¿ istnieje!";
            }
            labeltext = "";
            co = getUser(s);
            StateHasChanged();
            var r = await client.joinAsync(new User { Id = "777", Name = nick });

            var streamingCall = client.receiveMsg(new Empty { });
            but2Dis = true;
            but1Dis = false;
            //var reply0 = client.joinAsync(new User { Id = "001", Name = "Zenon" });
            //var reply01 = client.getAllUsers(new Empty { });
            //var reply01 = await client.getAllUsersAsync(new Empty { });
            //string s = reply01.Users.ToString();// ResponseAsync.Result.Users.ToString();
            // co = getUser(s);
            StateHasChanged();
            while (true)
            {
                await foreach (var ChatMessage in streamingCall.ResponseStream.ReadAllAsync())
                {
                    //var Chat =  streamingCall.ResponseStream.ReadAllAsync();

                    string wiad = ChatMessage.Msg;
                    string od = ChatMessage.From;
                    string czas = ChatMessage.Time;
                    // Console.WriteLine($"{ChatMessage.Time} | {ChatMessage.From} | {ChatMessage.Msg} C");
                    string imageSource = "logo.png";
                    string s1 = czas + ":" + "<b>[" + od + "]</b>" + "<br>" + wiad + '\n' + "<br>"; //+ "<img src=@imageSource>";
                    ss = ss + s1;
                    StateHasChanged();
                }
                Thread.Sleep(500);


            }


        }
        protected async System.Threading.Tasks.Task Button0Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            if (panelUsers == false)
            {
                button0text = "-";
                panelUsers = true;
            }
            else
            {
                panelUsers = false;
                button0text = "+";
            }
        }
    }   
}