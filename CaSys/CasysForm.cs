// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="CasysForm.cs" company="Nextsense">
//   © 2010 Nextsense
// </copyright>
// <summary>
//   Defines the CasysForm type.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CaSys
{
    public class CasysForm : WebControl, INamingContainer
    {
        private readonly Panel _firstPanel = new Panel();
        private readonly Panel _secondPanel = new Panel();
        private readonly Panel _thirdPanel = new Panel();
        public static NameValueCollection RequaredFields = new NameValueCollection();
        public static NameValueCollection OptionalFields = new NameValueCollection();
        private readonly Button _button = new Button();
        private readonly ImageButton _imgButton = new ImageButton();
        private readonly LinkButton _linkButton = new LinkButton();
        private TextBox _textBox = new TextBox();
        private Label _textBoxLabel = new Label();
        private Literal _lDiv = new Literal();
        public static Casys Casys;

        public bool Required
        {
            get
            {
                if (ViewState["Required"] == null)
                    ViewState.Add("Required", false);

                return (bool)ViewState["Required"];
            }
            set { ViewState["Required"] = value; }
        }

        public bool UseLayout
        {
            get
            {
                if (ViewState["UseLayout"] == null)
                    ViewState.Add("UseLayout", false);

                return (bool)ViewState["UseLayout"];
            }
            set { ViewState["UseLayout"] = value; }
        }

        public string ButtonText
        {
            get
            {
                if (ViewState["ButtonText"] == null)
                    ViewState.Add("ButtonText", string.Empty);

                return (string)ViewState["ButtonText"];
            }
            set { ViewState["ButtonText"] = value; }
        }

        public string ImageUrl
        {
            get
            {
                if (ViewState["ImageUrlV"] == null)
                    ViewState.Add("ImageUrlV", string.Empty);

                return (string)ViewState["ImageUrlV"];
            }
            set { ViewState["ImageUrlV"] = value; }
        }

        private void GenerateRequaredBlock()
        {
            if (_firstPanel.HasControls()) return;

            _firstPanel.ID = "pRequaredBlock";
            _firstPanel.CssClass = "requaredBlock";
            RequaredFields.Clear();
            RequaredFields.Add("AmountToPay", Casys.AmountToPay);
            RequaredFields.Add("AmountCurrency", Casys.AmountCurrency);
            RequaredFields.Add("Details1", Casys.Details1);
            RequaredFields.Add("Details2", Casys.Details2);
            RequaredFields.Add("Details3", Casys.Details3);
            RequaredFields.Add("PayToMerchant", Casys.PayToMerchant);
            RequaredFields.Add("MerchantName", Casys.MerchantName);
            RequaredFields.Add("PaymentOKURL", Casys.PaymentOKURL);
            RequaredFields.Add("PaymentFailURL", Casys.PaymentFailURL);

            for (var i = 0; i < RequaredFields.Count; i++)
            {
                _textBox = new TextBox();
                _textBoxLabel = new Label();
                _lDiv = new Literal
                            {
                                ID = "lDivR" + i,
                                Text = "<div class='space'></div>"
                            };

                _textBoxLabel.ID = "label" + RequaredFields.GetKey(i);
                _textBoxLabel.Text = RequaredFields.GetKey(i);

                _textBox.ID = "txtBox" + RequaredFields.GetKey(i);
                _textBox.Text = RequaredFields.Get(i);

                _firstPanel.Controls.Add(_textBoxLabel);
                _firstPanel.Controls.Add(_textBox);
                _firstPanel.Controls.Add(_lDiv);
            }
        }

        private void GenerateOptionalBlock()
        {
            if (!_secondPanel.HasControls())
            {
                _secondPanel.ID = "pOptionalBlock";
                _secondPanel.CssClass = "optionalBlock";

                OptionalFields.Clear();
                OptionalFields.Add("FirstName", Casys.FirstName);
                OptionalFields.Add("LastName", Casys.LastName);
                OptionalFields.Add("Address", Casys.Address);
                OptionalFields.Add("City", Casys.City);
                OptionalFields.Add("Zip", Casys.Zip);
                OptionalFields.Add("Country", Casys.Country);
                OptionalFields.Add("Telephone", Casys.Telephone);
                OptionalFields.Add("Email", Casys.Email);

                for (var i = 0; i < OptionalFields.Count; i++)
                {
                    _textBox = new TextBox();
                    _textBoxLabel = new Label();
                    _lDiv = new Literal
                        {
                            ID = "lDivO" + i,
                            Text = "<div class='space'></div>"
                        };

                    _textBoxLabel.ID = "label" + OptionalFields.GetKey(i);
                    _textBoxLabel.Text = OptionalFields.GetKey(i);

                    _textBox.ID = "txtBox" + OptionalFields.GetKey(i);
                    _textBox.Text = OptionalFields.Get(i);

                    _secondPanel.Controls.Add(_textBoxLabel);
                    _secondPanel.Controls.Add(_textBox);
                    _secondPanel.Controls.Add(_lDiv);
                }
            }
        }

        private void GenerateSubmitBlock(string submitControl)
        {
            if (_thirdPanel.HasControls()) return;

            _thirdPanel.CssClass = "submitBlock";

            switch (submitControl)
            {
                case "Button":
                    _button.ID = "submitButton";
                    _button.Text = ButtonText;
                    _button.OnClientClick = "this.disabled = true;";
                    _button.UseSubmitBehavior = false;
                    _button.Click += OnButton_Click;
                    _thirdPanel.Controls.Add(_button);
                    break;
                case "ImageButton":
                    _imgButton.ID = "submitImgButton";
                    _imgButton.ImageUrl = ImageUrl;
                    _imgButton.Click += _imgButton_Click;
                    _thirdPanel.Controls.Add(_imgButton);
                    break;
                case "LinkButton":
                    _linkButton.ID = "submitLinkButton";
                    _linkButton.Text = ButtonText;
                    _thirdPanel.Controls.Add(_linkButton);
                    break;
                default:
                    _button.ID = "submitButton";
                    _button.Text = ButtonText;
                    _button.OnClientClick = "this.disabled = true;";
                    _button.UseSubmitBehavior = false;
                    _button.Click += OnButton_Click;
                    _thirdPanel.Controls.Add(_button);
                    break;
            }
        }

        [Browsable(true)]
        public event EventHandler ImgClick;
        void _imgButton_Click(object sender, ImageClickEventArgs e)
        {
            if (ImgClick != null)
            {
                ImgClick(sender, e);
            }
            var remotePost = new RemotePost();
            remotePost.Post(Casys);
        }

        public void PostData(object sender, EventArgs e)
        {
            var remotePost = new RemotePost();
            remotePost.Post(Casys);
        }

        protected override void CreateChildControls()
        {
            if (UseLayout)
            {
                GenerateRequaredBlock();
                GenerateOptionalBlock();

                Controls.Add(_firstPanel);
                Controls.Add(_secondPanel);
            }

            GenerateSubmitBlock(Casys.SubmitControl);
            Controls.Add(_thirdPanel);

            base.CreateChildControls();
        }

        [Browsable(true)]
        public event EventHandler Click;
        protected virtual void OnButton_Click(object sender, EventArgs e)
        {
            if (Click != null)
            {
                Click(sender, e);
            }
            var remotePost = new RemotePost();
            remotePost.Post(Casys);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!UseLayout)
            {
                writer.AddAttribute("class", "divWrapper");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                _thirdPanel.RenderControl(writer);
                writer.RenderEndTag(); // end Div
            }
            else
            {
                writer.AddAttribute("class", "divWrapper");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                _firstPanel.RenderControl(writer);
                _secondPanel.RenderControl(writer);
                _thirdPanel.RenderControl(writer);
                writer.RenderEndTag(); // end Div
            }
        }
    }
}
