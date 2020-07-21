<%@ Page Language="C#" %>

<script runat="server">
    /// <summary>
    /// Calls setaffiliate and pass the query string plus the referrer.
    /// </summary>
    /// <returns></returns>
    public string SetAffiliate() {
        StringBuilder Query = new StringBuilder();
        Query.Append(Request.Url.Query);
        if (Request.UrlReferrer != null) {
            if (Query.Length > 0)
                Query.Append("&");
            Query.Append("urlref=" + HttpUtility.UrlEncode(Request.UrlReferrer.Host));
        }

        if (Query.Length > 0)
            return string.Format("<iframe style='width:0px;height:0px;border:0px' src='http://www.spiritualselftransformation.com/setaffiliate{0}'></iframe>", Query.ToString());
        else
            return "";
    }

    public string GetRef() {
        // Ref 7 = forceoflife.com
        string Ref = Request.Params["r"];
        int CodeRef = -1;
        if (!string.IsNullOrEmpty(Ref) && Ref.Length <= 2)
            int.TryParse(Ref, out CodeRef);
        if (CodeRef > -1)
            return "7" + CodeRef.ToString("00");
        else
            return "700";
    }
</script>
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
    <meta property="og:title" content="Live a Life You Love" />
    <meta property="og:image" content="http://www.spiritualselftransformation.com/images/fb-force-of-life.jpg" />
    <link rel="stylesheet" type="text/css" href="http://www.spiritualselftransformation.com/css/style.css" />
    <script type="text/javascript" src='http://www.spiritualselftransformation.com/js/jquery-1.9.1.min.js'></script>
    <script type="text/javascript" src="http://www.spiritualselftransformation.com/js/helper.js"></script>
    <%--    <script type="text/javascript">
        var allowExit = false;
        var exitMessage = "";  //"WAIT!\n\nThis might be the exact information you need to move your life forward right now.\n\nAren't you curious? Click 'Stay On This Page'.";
        function exitConfirm() {
            if (!allowExit && exitMessage != "")
                return exitMessage;
        }
    </script>--%>
    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-19070885-1']);
        _gaq.push(['_setDomainName', 'none']);
        _gaq.push(['_setAllowLinker', true]);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
    </script>
</head>
<body>
    <div id="fb-root">
    </div>
    <script>
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=124759651007662";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    </script>
    <div id="pagelanding">
        <div id="wrapper">
            <div class="header">
                <a href="~/" class="logo" runat="server">
                    <img class="logoimg" src="http://www.spiritualselftransformation.com/images/logo-header.png" height="84" width="84" alt="Spiritual Self Transformation" runat="server" />
                    <h1 class="logotext">Spiritual Self&nbsp;Transformation</h1>
                </a>
                <div class="rightheader">
                    <div class="tag">
                        Get The Life You Want
                    </div>
                </div>
                <div style="font: 13px Arial, Helvetica, sans-serif; float: right; padding-right: 10px;">Already registered? <a href="http://www.forceoflife.net/training/">Sign in</a></div>
            </div>
            <div class="middleDiv">
                <div class="contentPanel alpha">
                    <div style="height: 70px">
                        <div class="fb-like" data-href="https://www.facebook.com/consciousevo" data-send="true"
                            data-width="450"
                            data-show-faces="true" data-font="arial">
                        </div>
                    </div>
                    <div class="h1">
                        &ldquo;You Know You Have a Higher Purpose You Are Not Living&rdquo;
                    </div>
                    <div class="productRow">
                        <div>
                            <p class="blue">You're frustrated! And it's building into anger and depression up to a point where
                                your
                                life collapses. It doesn't have to be that way. You can make a different choice.
                            </p>
                            <p>The Force of Life is your emotional drive and power, the fuel powering joy, creativity
                                and passion.
                            </p>
                            <p>Sign up for the FREE <strong><i>Force of Life Monthly Training</i></strong> to
                            </p>
                            <ul class="checklist">
                                <li>See what is keeping you stuck so that you can turn the situation around</li>
                                <li>Understand the true power hidden within you</li>
                                <li>Open the door to stepping into your greater path, purpose and freedom</li>
                            </ul>
                            <img src="http://www.shamanicattraction.com/images/arrowdownleft.gif" width="102"
                                height="247" alt="Sign up here"
                                style="float: right; padding-top: 20px;" />
                            <p>Imagine waking up in the morning not because you have to work, but because <strong>there
                                    is so much you
                                want to do</strong>. You are in constant joy and happiness and unconditionally attract
                                people and opportunities
                                matching that vibe. <strong>You always know what decisions to make</strong> because
                                you have a strong
                                sense of purpose and guidance. This is the foundation to build a life you truly
                                love.
                            </p>
                            <p>Simply enter your name and email in the box below. You will also receive the latest
                                articles.<br />
                                We have a zero-spam policy and we will not share your information with anyone.
                            </p>
                        </div>

                        <center class="actionbox">
                            <form action="https://forms.ontraport.com/v2.4/form_processor.php?" method="post" accept-charset="UTF-8">
                            <input name="uid" type="hidden" value="p2c20557f2"/>
                            <table border="0" cellspacing="0" cellpadding="0" class="form">
                                <tr>
                                    <td>Name: </td>
                                    <td><input type="text" name="firstname" class="input" style="width: 200px" required />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Email: </td>
                                    <td><input type="email" name="email" class="input" style="width: 200px" required />
                                    </td>
                                </tr>
                            </table>
                            <div style="padding-top: 10px;">
                                <img src="http://www.shamanicattraction.com/images/lock.png" alt="lock" width="16" height="16" align="top" />
                                No spam. Unsubscribe any time.</div>
                            <input class="submitbox" name="signup" type="submit" value="Sign Me Up!" onclick="allowExit = true" />
                            </form>
                        </center>
                    </div>
                </div>
                <div style="float: right;">
                    <div class="sidebartop">
                        &nbsp;
                    </div>
                    <div class="sidebar">
                        <h2>Video Testimonials</h2>
                    </div>
                    <div class="sidebarbg">
                        <iframe width="220" height="128" src="http://www.youtube.com/embed/2UXy2jO2sDg" frameborder="0"
                            allowfullscreen>
                        </iframe>
                        <div style="height: 10px;">
                        </div>
                        <iframe width="220" height="166" src="http://www.youtube.com/embed/VLIA68d0cik" frameborder="0"
                            allowfullscreen>
                        </iframe>
                        <div style="height: 10px;">
                        </div>
                        <iframe width="220" height="166" src="http://www.youtube.com/embed/boadR58S_nA" frameborder="0"
                            allowfullscreen>
                        </iframe>
                    </div>
                    <div class="sidebarbottom">
                        &nbsp;
                    </div>
                    <%= SetAffiliate() %>
                </div>
            </div>
        </div>
    </div>
    <div id="footerarealanding">
        <div id="footerlanding">
            <div class="footerleftlanding">
                <div class="footerlogo">
                    <a href="http://www.spiritualselftransformation.com" runat="server" onclick="return OpenLink(this);">
                        <img src="http://www.spiritualselftransformation.com/images/footer-logo.png" alt="Spiritual Self Transformation"
                            runat="server" style="position: relative; left: -10px;" /></a>
                </div>
            </div>
            <div class="clear">
            </div>
            <div class="copyright" style="margin-top: 0px">
                Copyright &copy; <a href="http://www.spiritualselftransformation.com" runat="server"
                    target="_top" onclick="return OpenLink(this);">Spiritual Self Transformation</a>
                2008-2013. All Right Reserved.
            </div>
        </div>
    </div>
</body>
</html>
