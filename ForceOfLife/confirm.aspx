<%@ Page Language="C#" %>
<%@ Register Src="usercontrols/ViralShare.ascx" TagName="ViralShare" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <title>Live a Life You Love</title>
    <meta name="description" content="You're frustrated! And it's building into anger and depression up to a point where your life collapses. It doesn't have to be that way. You can make a different choice." />
    <meta name="keywords" content="life force, life purpose, energy healing, spirituality, purpose, life, love, universe" />
    <meta property="og:site_name" content="Force of Life" />
    <meta property="og:type" content="website" />
    <meta property="fb:admins" content="726296462" />
    <meta property="og:title" content="Live a Life You Love and Step Into Your Higher Purpose" />
    <meta property="og:image" content="https://www.spiritualselftransformation.com/images/logo-etienne.jpg" />
    <link rel="stylesheet" type="text/css" href="https://www.spiritualselftransformation.com/css/style.css" />
    <link rel="stylesheet" type="text/css" href="https://www.spiritualselftransformation.com/css/theme-overcast/jquery-ui-1.10.0.custom.min.css" />
    <script type="text/javascript" src="https://www.spiritualselftransformation.com/js/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="https://www.spiritualselftransformation.com/js/jquery-ui-1.10.0.custom.min.js"></script>
    <script type="text/javascript" src="https://www.spiritualselftransformation.com/js/helper.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="fb-root">
    </div>
    <script>
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=124759651007662";
            fjs.parentNode.insertBefore(js, fjs);
        } (document, 'script', 'facebook-jssdk'));
    </script>
    <div id="pagelanding">
        <div id="wrapper">
            <div class="header">
                <a href="~/" class="logo" runat="server">
                    <img class="logoimg" src="https://www.spiritualselftransformation.com/images/logo-header.png" height="84" width="84" alt="Spiritual Self Transformation" runat="server" />
                    <h1 class="logotext">Spiritual Self&nbsp;Transformation</h1>
                </a>
                <div class="rightheader">
                    <div class="tag">
                        Get The Life You Want</div>
                </div>
                <div style="font: 13px Arial, Helvetica, sans-serif; float:right; padding-right: 10px;">Already registered? <a href="https://www.forceoflife.net/training/">Sign in</a></div>
            </div>
            <div class="middleDiv">
                <div class="contentPanel alpha">
                    <div style="height: 70px">
                        <div class="fb-like" data-href="https://www.facebook.com/consciousevo" data-send="true" data-width="450"
                            data-show-faces="true" data-font="arial">
                        </div>
                    </div>
                    <div class="productRow">
                        <h1>Congratulations, you are now registered!</h1>
                        <p>You will receive the access information by email.</p>
                        <p>IMPORTANT: If you don't see the confirmation email, check your spam folder, mark the email as 'not junk'
                            and add 'etienne@spiritualselftransformation' to your white list. <a href="https://www.spiritualselftransformation.com/whitelist"
                                runat="server" target="_blank">Here are instructions on how to whitelist.</a></p>
                        <p>&nbsp;</p>
                        <p class="blue"><strong>BONUS #1: <i>Results-Oriented Spirituality</i> ebook</strong></p>
                        <p>Share with 3 friends to receive this ebook and <strong>use spirituality to achieve concrete results</strong>
                            ($97 value).</p>
                        <p class="blue"><strong>BONUS #2: <i>7 Power Secrets to Design a Lifestyle of Freedom</i> recording</strong></p>
                        <p>Share with 5 friends and also receive this teleconference recording to <strong>design a lifestyle of
                            freedom</strong> ($97 value).</p>
                        <uc1:ViralShare ID="ViralShare1" runat="server" BonusTitle="Results-Oriented Spirituality" BonusUrl="https://www.spiritualselftransformation.com/files/results-oriented-spirituality.pdf"
                            EmailTitle="Force of Life" EmailContent="ViralShareEmailForceOfLife">
                            <Bonus2Template>
                                <p><strong>Here are your bonuses:</strong><br />
                                    <a target="_blank" href="https://www.spiritualselftransformation.com/files/results-oriented-spirituality.pdf">Bonus #1:
                                        Results-Oriented Spirituality</a><br />
                                    <a target="_blank" href="https://www.spiritualselftransformation.com/files/design-lifestyle-freedom.mp3">Bonus #2: 7 Power Secrets to Design a Lifestyle of Freedom</a></p>
                            </Bonus2Template>
                        </uc1:ViralShare>
                        <p>&nbsp;</p>
                        <p style="margin-left: 40px;">
                            <img src="https://www.spiritualselftransformation.com/images/sign.jpg" runat="server" hspace="10" height="164"
                                align="left" width="394" style="padding-right: 5px; float: none;" alt="To your freedom" /></p>
                    </div>
                </div>
                <div style="float: right;">
                    <div class="sidebartop">
                        &nbsp;</div>
                    <div class="sidebar">
                        <h2>Video Testimonials</h2>
                    </div>
                    <div class="sidebarbg">
                        <iframe width="220" height="128" src="//www.youtube-nocookie.com/embed/2UXy2jO2sDg?rel=0;showinfo=0" frameborder="0" allowfullscreen></iframe>
                        <div style="height: 10px;">
                        </div>
                        <iframe width="220" height="166" src="//www.youtube-nocookie.com/embed/VLIA68d0cik?rel=0;showinfo=0" frameborder="0" allowfullscreen></iframe>
                        <div style="height: 10px;">
                        </div>
                        <iframe width="220" height="166" src="//www.youtube-nocookie.com/embed/boadR58S_nA?rel=0;showinfo=0" frameborder="0" allowfullscreen></iframe>
                    </div>
                    <div class="sidebarbottom">
                        &nbsp;</div>
                </div>
            </div>
        </div>
    </div>
    <div id="footerarealanding">
        <div id="footerlanding">
            <div class="footerleftlanding">
                <div class="footerlogo">
                    <a href="https://www.spiritualselftransformation.com" runat="server" onclick="return OpenLink(this);">
                        <img src="https://www.spiritualselftransformation.com/images/footer-logo.png" alt="Spiritual Self Transformation"
                            runat="server" style="position: relative; left: -10px;" /></a>
                </div>
            </div>
            <div class="clear">
            </div>
            <div class="copyright" style="margin-top: 0px">
                Copyright &copy; <a href="https://www.spiritualselftransformation.com" runat="server" target="_top">
                    Spiritual Self Transformation</a> 2008-2014. All Right Reserved.</div>
        </div>
    </div>
    </form>
</body>
</html>
