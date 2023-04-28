using MyBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyBot
{
    public partial class Form1 : Form
    {
        private Thread botThread;
        private static string Token = "5771983024:AAHrKfAH9EBwl8vekjeeJ1AF__lnrMscFMQ";
        private Telegram.Bot.TelegramBotClient bot;
        private ReplyKeyboardMarkup mainKeyboardMarkup;

        private int bale1, bale2, bale3 = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            Token = txtToken.Text;
            botThread = new Thread(new ThreadStart(RunRobot));
            botThread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mainKeyboardMarkup = new ReplyKeyboardMarkup();
            KeyboardButton[] row1 =
            {
                new KeyboardButton("\U0000269C " + "بیت پرداخت" + " \U0001F531"),
                new KeyboardButton("\U0001F530 " + "لیست صرافی ها" + " \U0001F530"),
            };
            KeyboardButton[] row2 =
            {
                new KeyboardButton("\U0001F4A1 " + "درباره ما" + " \U0001F4A1"),
                new KeyboardButton("\U0001F4C8 " + "قیمت لحظه ای" + " \U0001F4C9")
            };
            mainKeyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                row1,
                row2
            };
        }
        void RunRobot()
        {
            bot = new Telegram.Bot.TelegramBotClient(Token);
            this.Invoke(new Action(() =>
            {
                lblStatus.Text = "Online";
                lblStatus.ForeColor = Color.Green;
            }));
            int offset = 0;
            try
            {
                while (true)
                {
                    Telegram.Bot.Types.Update[] updates = bot.GetUpdatesAsync(offset).Result;
                    foreach (var update in updates)
                    {
                        offset = update.Id + 1;

                        if (update.Message == null)
                            continue;
                        var text = update.Message.Text?.ToLower();
                        if (text is null) throw new Exception();

                        var from = update.Message.From;
                        var chatId = update.Message.Chat.Id;
                        if (text.Contains("/start"))
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("سلام " + "\U0001F60A");
                            sb.AppendLine("به ربات بیت پرداخت خوش آمدید" + " \U0001F916");
                            sb.AppendLine("برای استفاده از امکانات ربات از دکمه های مربوطه اقدام کنید");
                            bot.SendTextMessageAsync(chatId, sb.ToString(), ParseMode.Default, false, false, 0, mainKeyboardMarkup);
                        }
                        else if (text.Contains("لیست صرافی ها"))
                        {
                            var sb = new StringBuilder();
                            var prices = GetPrices().Result;
                            #region ButtonsList 
                            //var inline = new InlineKeyboardMarkup();
                            //var inlineKeyBoards = Buttons(prices);

                            //foreach (var item in inlineKeyBoards)
                            //{
                            //    inline.InlineKeyboard = new InlineKeyboardButton[][]
                            //    {
                            //    inlineKeyBoards.Select(x=>x[0]).Take(2).ToArray(),
                            //    inlineKeyBoards.Select(x=>x[0]).Skip(2).ToArray(),
                            //    };
                            //}
                            #endregion
                            sb.AppendLine("\U0001F530 " + "لیست صرافی ها" + " \U0001F530");
                            sb.AppendLine($"");
                            foreach (var price in prices)
                            {
                                sb.AppendLine("\U0001F530 " + $"عنوان صرافی : {price.title}");
                                sb.AppendLine("\U0001F4B0 " + $"مبلغ خرید : {price.buy:0,#} تومان");
                                sb.AppendLine("\U0001F4B0 " + $"مبلغ فروش : {price.sell:0,#} تومان");
                                sb.AppendLine("\U000026A0 " + $"کارمزد : {price.tax}");
                                sb.AppendLine($"");
                            }
                            InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
                            InlineKeyboardUrlButton[] row1 =
                            {
                            new InlineKeyboardUrlButton("بیت پرداخت","http://31.40.4.89:5000")
                        };
                            inline.InlineKeyboard = new InlineKeyboardButton[][]
                            {
                            row1
                            };
                            bot.SendTextMessageAsync(chatId, sb.ToString(), ParseMode.Default, false, false, 0, inline);
                        }
                        else if (text.Contains("/address") || text.Contains("بیت پرداخت"))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(@"http://31.40.4.89:5000");
                            InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
                            InlineKeyboardUrlButton[] row1 =
                            {
                            new InlineKeyboardUrlButton("\U0001F310 " + "بیت پرداخت","http://31.40.4.89:5000")
                        };
                            inline.InlineKeyboard = new InlineKeyboardButton[][]
                            {
                            row1
                            };
                            bot.SendTextMessageAsync(chatId, sb.ToString(), ParseMode.Default, false, false, 0, inline);
                        }
                        else if (text.Contains("درباره ما"))
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("\U0001F4A1 " + "درباره ما" + " \U0001F4A1");
                            sb.AppendLine($"");
                            sb.AppendLine("\U00002705 " + "بیت پرداخت ، پلتفرم  مقایسه ی جامع قیمت تتر (Tether) در صرافی های مختلف ارزهای دیجیتال ایرانی را ارائه می دهد.");
                            sb.AppendLine("کاربران به راحتی می توانند قیمت های خرید و فروش فعلی تتر را در صرافی های مختلف و همچنین تغییر قیمت و حجم معاملات 24 ساعته را مشاهده کنند.");
                            sb.AppendLine("علاوه بر این، پلتفرم ما به کاربران اجازه می دهد تا مبادلات مورد علاقه خود را دنبال کنند .");
                            bot.SendTextMessageAsync(chatId, sb.ToString(), ParseMode.Default, false, false, 0, mainKeyboardMarkup);
                        }
                        else if (text.Contains("قیمت لحظه ای"))
                        {
                            var sb = new StringBuilder();
                            var prices = GetPrices().Result;
                            sb.AppendLine("\U0001F4C8 " + "قیمت لحظه ای" + " \U0001F4C9");
                            sb.AppendLine($"");
                            foreach (var price in prices.Take(4))
                            {
                                sb.AppendLine("\U0001F530 " + $"عنوان صرافی : {price.title}");
                                sb.AppendLine("\U0001F4B0 " + $"مبلغ خرید : {price.buy:0,#} تومان");
                                sb.AppendLine("\U0001F4B0 " + $"مبلغ فروش : {price.sell:0,#} تومان");
                                sb.AppendLine("\U000026A0 " + $"کارمزد : {price.tax}");
                                sb.AppendLine($"");
                            }
                            InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
                            InlineKeyboardUrlButton[] row1 =
                            {
                            new InlineKeyboardUrlButton("بیت پرداخت","http://31.40.4.89:5000")
                        };
                            inline.InlineKeyboard = new InlineKeyboardButton[][]
                            {
                            row1
                            };
                            bot.SendTextMessageAsync(chatId, sb.ToString(), ParseMode.Default, false, false, 0, inline);
                        }

                        dgReport.Invoke(new Action(() =>
                        {
                            dgReport.Rows.Add(chatId, from.Username, text, update.Message.MessageId, update.Message.Date.ToString("yyyy/MM/dd - HH:mm"));
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                RunRobot();
            }
        }

        static InlineKeyboardUrlButton[] To1DArray(InlineKeyboardUrlButton[,] input)
        {
            // Step 1: get total size of 2D array, and allocate 1D array.
            int size = input.Length;
            InlineKeyboardUrlButton[] result = new InlineKeyboardUrlButton[size];

            // Step 2: copy 2D array elements into a 1D array.
            int write = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                for (int z = 0; z <= input.GetUpperBound(1); z++)
                {
                    result[write++] = input[i, z];
                }
            }
            // Step 3: return the new array.
            return result;
        }
        public List<InlineKeyboardUrlButton[]> Buttons(List<PriceResultDto> prices)
        {
            var inlineKeyBoards = new List<InlineKeyboardUrlButton[]>();
            foreach (var price in prices)
            {
                InlineKeyboardUrlButton[] row = { new InlineKeyboardUrlButton(price.title, price.Link) };
                inlineKeyBoards.Add(row);
            }
            return inlineKeyBoards;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            botThread.Abort();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (dgReport.CurrentRow != null)
            {
                int chatId = int.Parse(dgReport.CurrentRow.Cells[0].Value.ToString());
                bot.SendTextMessageAsync(chatId, txtMessage.Text, ParseMode.Html, true);
                txtMessage.Text = "";
            }
        }
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFile.FileName;
            }
        }
        private void btnPhoto_Click(object sender, EventArgs e)
        {
            if (dgReport.CurrentRow != null)
            {
                int chatId = int.Parse(dgReport.CurrentRow.Cells[0].Value.ToString());
                FileStream imageFile = System.IO.File.Open(txtFilePath.Text, FileMode.Open);
                bot.SendPhotoAsync(chatId, new FileToSend("1234.jpg", imageFile), txtMessage.Text);
            }
        }

        private void btnVideo_Click(object sender, EventArgs e)
        {
            if (dgReport.CurrentRow != null)
            {
                int chatId = int.Parse(dgReport.CurrentRow.Cells[0].Value.ToString());
                FileStream videoFile = System.IO.File.Open(txtFilePath.Text, FileMode.Open);
                bot.SendVideoAsync(chatId, new FileToSend("iman.mp4", videoFile));
            }
        }

        private void btnSendText_Click(object sender, EventArgs e)
        {
            bot.SendTextMessageAsync(txtChannel.Text, txtMessage.Text, ParseMode.Html);
        }

        private void txtToken_TextChanged(object sender, EventArgs e) { }

        public async Task<List<PriceResultDto>> GetPrices()
        {
            var result = new List<PriceResultDto>();
            try
            {
                using (var client = new WebClient { Encoding = Encoding.UTF8 })
                {
                    var json = await client.DownloadStringTaskAsync("http://31.40.4.89:3000/api/prices/GetPrices");
                    result = JsonConvert.DeserializeObject<List<PriceResultDto>>(json);
                }
                return result;
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return null;
        }

        private void btnSendPhoto_Click(object sender, EventArgs e)
        {
            FileStream imageFile = System.IO.File.Open(txtFilePath.Text, FileMode.Open);
            bot.SendPhotoAsync(txtChannel.Text, new FileToSend("1234.jpg", imageFile), txtMessage.Text);
        }

        private void btnSendVideo_Click(object sender, EventArgs e)
        {
            FileStream videoFile = System.IO.File.Open(txtFilePath.Text, FileMode.Open);
            bot.SendVideoAsync(txtChannel.Text, new FileToSend("iman.mp4", videoFile));
        }
    }
}
