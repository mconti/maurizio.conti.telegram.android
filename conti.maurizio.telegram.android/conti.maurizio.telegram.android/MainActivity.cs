using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Timers;
using Telegram.Bot.Types.Enums;

namespace conti.maurizio.telegram.android
{
    [Activity(Label = "Android Bot", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        Telegram.Bot.Api Bot { get; set; }
        int offset = 0;
        long chatId = 239121645;
        bool stato = false;
        Timer timer;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Bot = new Telegram.Bot.Api("");

            timer = new Timer();
            timer.Interval = 500;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.BottoneCount);
            button.Click += Button_Click;
            Button bottoneGo = FindViewById<Button>(Resource.Id.bottoneGo);
            bottoneGo.Click += BottoneGo_Click;

        }

        private async void BottoneGo_Click(object sender, EventArgs e)
        {
            var lblMe = FindViewById<TextView>(Resource.Id.lblMe);
            try
            {
                if (chatId != 0)
                {
                    var result = await Bot.SendTextMessageAsync(chatId, "/status");
                    lblMe.Text = result.MessageId.ToString();
                }
            }
            catch (Exception Err)
            {
                lblMe.Text = Err.Message;
            }
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        private async void Button_Click(object sender, EventArgs e)
        {

            try
            {
                //log.Items.Add("me.FirstName: " + me.FirstName);

                var updates = await Bot.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    switch (update.Type)
                    {
                        case UpdateType.MessageUpdate:
                            var message = update.Message;

                            // Memorizza il chat id
                            chatId = message.Chat.Id;

                            switch (message.Type)
                            {
                                case MessageType.TextMessage:

                                    switch (message.Text)
                                    {
                                        case "/start":
                                            break;

                                        case "/toggle":
                                            stato = ToggleLED(stato);

                                            if (stato)
                                                await Bot.SendTextMessageAsync(message.Chat.Id, $"Grazie {message.From.Username} per aver Acceso il faretto FLR", replyToMessageId: message.MessageId);
                                            else
                                                await Bot.SendTextMessageAsync(message.Chat.Id, $"Grazie {message.From.Username} per aver Spento il faretto FLR", replyToMessageId: message.MessageId);

                                            break;

                                        case "/status":
                                            if (stato)
                                                await Bot.SendTextMessageAsync(message.Chat.Id, "Acceso", replyToMessageId: message.MessageId);
                                            else
                                                await Bot.SendTextMessageAsync(message.Chat.Id, "Spento", replyToMessageId: message.MessageId);
                                            break;

                                        default:
                                            string helpMessage = $"{message.Text} ??\nComandi disponibili:\n/status\n/toggle\n";
                                            await Bot.SendTextMessageAsync(message.Chat.Id, helpMessage, replyToMessageId: message.MessageId);
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }

                    offset = update.Id + 1;
                }
            }
            catch (Exception err)
            {
                //log.Items.Add("Err: " + err.Message);
            }

            // Aggiorno i vari ID.
            var lblMe = FindViewById<TextView>(Resource.Id.lblMe);
            try
            {
                var me = await Bot.GetMeAsync();
                lblMe.Text = "Hi! I'm " + me.Username;
            }
            catch (Exception Err)
            {
                lblMe.Text = Err.Message;
            }

            Button button = FindViewById<Button>(Resource.Id.BottoneCount);
            button.Text = $"last id: {offset}, click: {count++}, id: {chatId}";
        }

        private bool ToggleLED(bool stato)
        {
            //if (stato)
            //btn.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
            //else
            //btn.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));

            return !stato;
        }
    }
}

