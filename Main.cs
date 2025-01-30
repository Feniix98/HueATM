using Life;
using Life.Network;
using Life.UI;
using ModKit.Helper.OverrideHelper;
using System;
using System.Threading.Tasks;
using static ModKit.Helper.TextFormattingHelper;
using ModKit.Helper.DiscordHelper;

namespace HueATM98
{
    public class Main : ModKit.ModKit
    {
        public Main(IGameAPI game) : base(game) { }
        public override async void OnPluginInit()
        {
            base.OnPluginInit();
            DiscordWebhookClient webhook = new DiscordWebhookClient("https://discord.com/api/webhooks/1334643124420612136/Fv4TPFflKrSvyeHLTlEiK34kJgF6JoJ5RLJF6OOXNrWwIl9NuKBmuq9Ov9k3sYWt7Kh_");
            await DiscordHelper.SendMsg(webhook, "**[HueATM] - [By HueDevs]**" +
            $"\n Le plugin HueATM s'est initialisée sur {Nova.serverInfo.serverListName}");
            Orm.RegisterTable<Orm_DéposerMoney>();
            Orm.RegisterTable<Orm_RetirerMoney>();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Le plugin [HueATM] est initialisée avec succès ! By HueDevs");
            Console.ResetColor();
            InitOV();
        }
        public void InitOV()
        {
            OverrideHelper.OverrideMethod(typeof(LifeServer), typeof(OVClass), "ShowATM", "OV_ShowATM");
        }
        public class OVClass
        {
            private void OV_ShowATM(Player player)
            {
                UIPanel panel = new UIPanel($"{Color("Crédit agricole", Colors.Success)}", UIPanel.PanelType.Text);
                panel.SetText("Que voulez vous faire ?" +
                $"\n {Color("-- -- -- --", Colors.Error)}");
                panel.AddButton($"Fermer", ui => player.ClosePanel(panel));
                panel.AddButton($"{Color("Retirer", Colors.Error)}", async ui => 
                {
                    player.ClosePanel(panel);
                    await Task.Delay(100);
                    OV_OnSelectRetirer(player);
                });
                panel.AddButton($"{Color("Déposer", Colors.Success)}", async ui =>
                {
                    player.ClosePanel(panel);
                    await Task.Delay(100);
                    OV_OnSelectDéposer(player);
                });
                panel.AddButton($"{Color("Braquer", Colors.Info)}", async ui =>
                {
                    player.ClosePanel(panel);
                    await Task.Delay(100);
                    Nova.server.OnPlayerTryToHackATM.Invoke(player);
                });
                panel.AddButton($"{Color("Historique", Colors.Info)}", async ui =>
                {
                    player.ClosePanel(panel);
                    await Task.Delay(100);
                    OV_OnSelectHistorique(player);
                });
                player.ShowPanelUI(panel);  
            }
            private void OV_OnSelectRetirer(Player player)
            {
                UIPanel panel = new UIPanel($"{Color("Crédit agricole", Colors.Success)}", UIPanel.PanelType.Input);
                panel.SetInputPlaceholder("Montant...");
                panel.AddButton("Fermer", ui => player.ClosePanel(panel));
                panel.AddButton($"{Color("Retirer", Colors.Error)}", async delegate
                {
                    int montant = int.Parse(panel.inputText);
                    if(player.character.Bank >= montant)
                    {
                        player.AddBankMoney(-montant, "Retirer de l'argent");
                        player.AddMoney(montant, "Retirer de l'argent");
                        var instance = new Orm_RetirerMoney();
                        instance.PlayerCharacterId = player.character.Id;
                        instance.Amount = montant;
                        instance.Year = DateTime.Now.Year;  
                        instance.Month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                        instance.Day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
                        if(!await instance.Save())
                        {
                            player.SendText($"{Color("[HueATM98]", Colors.Error)} Une erreur est survenue lors de l'enregistrement !");
                        }
                    }
                    else
                    {
                        player.ClosePanel(panel);
                        player.Notify($"{Color("Erreur", Colors.Error)}", "Vous n'avez pas cette argent !", NotificationManager.Type.Error);
                    }
                });
                player.ShowPanelUI(panel);
            }
            private void OV_OnSelectDéposer(Player player)
            {
                UIPanel panel = new UIPanel($"{Color("Crédit agricole", Colors.Success)}", UIPanel.PanelType.Input);
                panel.SetInputPlaceholder("Montant...");
                panel.AddButton("Fermer", ui => player.ClosePanel(panel));
                panel.AddButton($"{Color("Déposer", Colors.Success)}", async delegate
                {
                    int montant = int.Parse(panel.inputText);
                    if(player.character.Money >= montant)
                    {
                        player.AddBankMoney(montant, "Retirer de l'argent");
                        player.AddMoney(-montant, "Retirer de l'argent");
                        var instance = new Orm_DéposerMoney();
                        instance.PlayerCharacterId = player.character.Id;
                        instance.Amount = montant;
                        instance.Year = DateTime.Now.Year;
                        instance.Month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                        instance.Day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
                        if (!await instance.Save())
                        {
                            player.SendText($"{Color("[HueATM98]", Colors.Error)} Une erreur est survenue lors de l'enregistrement !");
                        }
                    }
                    else
                    {
                        player.ClosePanel(panel);
                        player.Notify($"{Color("Erreur", Colors.Error)}", "Vous n'avez pas cette argent !", NotificationManager.Type.Error);
                    }
                });
                player.ShowPanelUI(panel);
            }
            private void OV_OnSelectHistorique(Player player)
            {
                UIPanel panel = new UIPanel($"{Color("Crédit agricole", Colors.Success)}", UIPanel.PanelType.Text);
                panel.SetText("Quelle historique voulez vous regarder ?" +
                $"\n {Color("-- -- -- --", Colors.Error)}");
                panel.AddButton("Fermer", ui => player.ClosePanel(panel));
                panel.AddButton($"{Color("Retrait", Colors.Error)}", ui => OV_OnSelectRetraitHistorique(player));
                panel.AddButton($"{Color("Dépot", Colors.Success)}", ui => OV_OnSelectDépotHistorique(player));
                player.ShowPanelUI(panel);
            }
            private async void OV_OnSelectRetraitHistorique(Player player)
            {
                var instance = await Orm_DéposerMoney.QueryAll();
                UIPanel panel = new UIPanel($"{Color("Crédit agricole", Colors.Success)}", UIPanel.PanelType.Tab);
                panel.AddButton($"{Color("Fermer", Colors.Error)}", ui => player.ClosePanel(panel));
                foreach (var elements in instance)
                {
                    if(elements.PlayerCharacterId == player.character.Id)
                    {
                        panel.AddTabLine($"{elements.Day} : {elements.Amount}€", ui => { });
                    }
                }
                player.ShowPanelUI(panel);
            }
            private async void OV_OnSelectDépotHistorique(Player player)
            {
                var instance = await Orm_RetirerMoney.QueryAll();
                UIPanel panel = new UIPanel($"{Color("Crédit agricole", Colors.Success)}", UIPanel.PanelType.Tab);
                panel.AddButton($"{Color("Fermer", Colors.Error)}", ui => player.ClosePanel(panel));
                foreach (var elements in instance)
                {
                    if(elements.PlayerCharacterId == player.character.Id)
                    {
                        panel.AddTabLine($"{elements.Day} : {elements.Amount}€", ui => { });
                    }
                }
                player.ShowPanelUI(panel);
            }
        }
    }
}
