<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Register Assembly="BusinessHelper" Namespace="BusinessHelper" TagPrefix="cc1" %>
<script runat="server">
    private string bonusTitle = "";
    private string bonusUrl = "";
    private string bonusType = "ebook";
    private string emailTitle = "";
    private string emailContent = "";

    [PersistenceModeAttribute(PersistenceMode.InnerProperty)]
    [TemplateInstanceAttribute(TemplateInstance.Single)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public ITemplate Bonus2Template { get; set; }

    protected override void OnLoad(EventArgs e) {
        if (txtYourName.Text.Length == 0)
            txtYourName.Text = Request.QueryString["firstname"];
        if (txtYourEmail.Text.Length == 0)
            txtYourEmail.Text = Request.QueryString["email"];
        int ShareCount = 0;
        if (Request.QueryString["sent"] != null)
            int.TryParse(Request.QueryString["sent"], out ShareCount);
        if (ShareCount > 0) {
            lblInvitationsSent.Visible = true;
            divShare.Visible = false;
            if (ShareCount >= 5 && this.Bonus2Template != null)
                this.Bonus2Template.InstantiateIn(divBonus);
        }
        base.OnLoad(e);
    }

    protected void cmdShare_Click(object sender, EventArgs e) {
        if (!Page.IsValid || txtYourName.Text.Trim().Length == 0 || txtYourEmail.Text.Trim().Length == 0)
            return;

        ViralShare Share = new ViralShare();
        Share.AddFriend(txtShareName1.Text, txtShareEmail1.Text);
        Share.AddFriend(txtShareName2.Text, txtShareEmail2.Text);
        Share.AddFriend(txtShareName3.Text, txtShareEmail3.Text);
        Share.AddFriend(txtShareName4.Text, txtShareEmail4.Text);
        Share.AddFriend(txtShareName5.Text, txtShareEmail5.Text);

        Share.Send(txtYourName.Text, txtYourEmail.Text, emailContent, emailTitle);

        txtShareName1.Text = "";
        txtShareName2.Text = "";
        txtShareName3.Text = "";
        txtShareName4.Text = "";
        txtShareName5.Text = "";
        txtShareEmail1.Text = "";
        txtShareEmail2.Text = "";
        txtShareEmail3.Text = "";
        txtShareEmail4.Text = "";
        txtShareEmail5.Text = "";

        if (Share.FriendCount >= 3 && (Share.FriendCount < 5 || Bonus2Template == null))
            Response.Redirect(bonusUrl);
        else
            Response.Redirect(CustomSiteMapPath.TrimUrl(Request.Url.AbsolutePath) + "?sent=" + Share.FriendCount);
    }

    [Browsable(true)]
    public string BonusTitle {
        get { return bonusTitle; }
        set { bonusTitle = value; }
    }

    [Browsable(true)]
    public string BonusUrl {
        get { return bonusUrl; }
        set { bonusUrl = value; }
    }

    [Browsable(true)]
    public string BonusType {
        get { return bonusType; }
        set { bonusType = value; }
    }

    [Browsable(true)]
    public string EmailTitle {
        get { return emailTitle; }
        set { emailTitle = value; }
    }

    [Browsable(true)]
    public string EmailContent {
        get { return emailContent; }
        set { emailContent = value; }
    }
</script>
<script type='text/javascript'>
    var DialogClick = false;

    function btnViralShare_Click() {
        if (DialogClick == true)
            return true;
        $('#dialog-block').dialog('open');
        return false;
    }

    function submitFromDialog() {
        DialogClick = true;
        $('#dialog-block').dialog('close');
        document.getElementById('<%= cmdShare.ClientID %>').click()
    }

    $(document).ready(function () {
        $('#dialog-block').dialog({
            autoOpen: false,
            modal: true,
            width: 400,
            open: function (event, ui) {
                var output = '';
                var emails = $('input[id*=\'txtShareEmail\']')
                var names = $('input[id*=\'txtShareName\']')
                var invitationCount = 0;
                var error = 0;
                var erroremail = 0;
                var friendname;
                var friendemail;
                var emailpattern = new RegExp(/^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/);

                friendname = $('#<%= txtYourName.ClientID %>').val().trim();
                friendemail = $('#<%= txtYourEmail.ClientID %>').val().trim();
                if (friendname == '') {
                    erroremail = 1;
                    output += '<br /> Please enter your name.';
                }
                if (friendemail == '' || emailpattern.test(friendemail) == false) {
                    erroremail = 1;
                    output += '<br /> Please enter your valid email.';
                }

                for (i = 0; i < emails.length; i++) {
                    friendname = $('#' + names[i].id).val().trim();
                    friendemail = $('#' + emails[i].id).val().trim();
                    if (friendemail != '') {
                        if (emailpattern.test(friendemail) == false) {
                            erroremail = 1;
                            output += '<br /> Email ' + friendemail + ' is invalid.';
                        }
                        else {
                            invitationCount += 1;
                            if (friendname == '') {
                                error = 1;
                                output += '<br /> You should specify a name for ' + friendemail;
                            }
                        }
                    }
                }
                if (output != '') { output += '<br />'; }

                if (invitationCount < 3) {
                    output += '<br />You must share with at least 3 friends to receive the <%= bonusType %> <strong><%= bonusTitle %></strong>.';
                    output += '<br /><br />Would you like to share the invitation with more friends?'
                }

                if (output == '') { submitFromDialog(); return true; }

                if (invitationCount > 0 && erroremail == 0) {
                    $('#dialog-block').dialog({ buttons: { 'Go Back': function () { $('#dialog-block').dialog('close') },
                        'Send Invitations': function () { submitFromDialog(); }
                    }
                    });
                } else {
                    $('#dialog-block').dialog({ buttons: { 'Go Back': function () { $('#dialog-block').dialog('close') } } });
                }

                $('#dialog-block').html(output)
            }
        });
    })
</script>
<asp:Literal ID="lblInvitationsSent" Visible="false" runat="server"><p class="blue" style="color:red;">Your invitations have been sent! :-)</p></asp:Literal>
<div id="divShare" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" class="form" style="text-align: center; padding-bottom: 10px;">
        <tr>
            <td>&nbsp; </td>
            <td width="160"><strong>Your name</strong> </td>
            <td width="210"><strong>Your email</strong> </td>
        </tr>
        <tr>
            <td>&nbsp; </td>
            <td>
                <asp:TextBox ID="txtYourName" Width="110" runat="server" class="input"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtYourEmail" Width="200" runat="server" class="input"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>&nbsp; </td>
            <td><strong>Friend's name</strong> </td>
            <td><strong>Friend's email</strong> </td>
        </tr>
        <tr>
            <td>1. </td>
            <td>
                <asp:TextBox ID="txtShareName1" Width="110" runat="server" class="input"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtShareEmail1" Width="200" runat="server" class="input"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>2. </td>
            <td>
                <asp:TextBox ID="txtShareName2" Width="110" runat="server" class="input"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtShareEmail2" Width="200" runat="server" class="input"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>3. </td>
            <td>
                <asp:TextBox ID="txtShareName3" Width="110" runat="server" class="input"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtShareEmail3" Width="200" runat="server" class="input"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>4. </td>
            <td>
                <asp:TextBox ID="txtShareName4" Width="110" runat="server" class="input"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtShareEmail4" Width="200" runat="server" class="input"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>5. </td>
            <td>
                <asp:TextBox ID="txtShareName5" Width="110" runat="server" class="input"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtShareEmail5" Width="200" runat="server" class="input"></asp:TextBox>
            </td>
        </tr>
    </table>
    <p class="blue">
        <asp:Button ID="cmdShare" runat="server" class="submitbox" Text="SHARE" OnClick="cmdShare_Click" OnClientClick="return btnViralShare_Click();" />&nbsp;
        &hellip;and download
        <%= bonusType %></p>
    <div id="dialog-block" style="display: none">
    </div>
</div>
<div id="divBonus" runat="server">
</div>
