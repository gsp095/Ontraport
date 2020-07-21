/*---------------------------
 LeadPages Custom Functions
 ----------------------------*/
var leadpages_input_data = {
};

$(function () {
	//variables are defined in the template.json file
	for (var key in leadpages_input_data) {
	    var url_variables = ['facebookurl', 'twitterurl', 'googleurl'];
	    if (url_variables.indexOf(key) !== -1) {
	        if (leadpages_input_data[key] === '') {
	            leadpages_input_data[key] = window.location.href;
	        }
	    }
	}
	;

	//leadpages_input_data variables come from the template.json "variables" section
	$('.pop-facebook').attr('href', "https://www.facebook.com/sharer/sharer.php?u=" + leadpages_input_data["facebookurl"]);
	$('.pop-google').attr('href', "https://plus.google.com/share?url=" + leadpages_input_data["googleurl"]);
	$('.pop-twitter').attr('href', "https://twitter.com/intent/tweet?url=" + leadpages_input_data["twitterurl"]);
	$('.fb-comments').attr('data-href', "" + leadpages_input_data['facebookcomments']);
	$('.fb-comments').attr('data-num-posts', "" + leadpages_input_data['facebookcommentsposts']);
});


/**
 * The following is a hack to get the builder to dynamically update the DOM when the progress bar related
 * dynamic type elements are updated. This also allows the progress bar to display the correct value when
 * the numbers or their container are not in the published version.
 */
$(function(){
	/**
	 * Get the passed page variable data object from the correct
	 * source based on whether in the builder or on published page.
	 *
	 * @param {strimng} pageVar - the page variable name
	 * @return {object} the variable object if it exists, else empty object
	 */
	function getPageVar(pageVar){
		return (window.top && typeof window.top.App !== 'undefined') ? window.top.App.Data.read('page.variables')[pageVar] : window.LeadPageData[pageVar] || {};
	}

	/**
	 * Get the passed page variable's value if set, else the default value.
	 *
	 * @param {strimng} pageVar - the page variable name
	 * @return {string} the variable value if it exists, else the variable default, else empty object
	 */
	function getPageVarValue(pageVar){
		return getPageVar(pageVar)['value'] || getPageVar(pageVar)['default'] || {};
	}

	/**
	 * Updates the DOM with the latest data for the progress counts.
	 */
	function updateProgressCountHTML(){
		if ( document.querySelector('.start') ){
			document.querySelector('.start').innerHTML = getPageVarValue('progressCurrent');
		}
		if ( document.querySelector('.end') ){
			document.querySelector('.end').innerHTML = getPageVarValue('progressTotal');
		}
	}

	/**
	 * Calculates the progress percentage based on latest data for the progress counts and updates the DOM.
	 */
	function calculatePercent() {
		var progress = getPageVarValue('progressCurrent');
		var	total = getPageVarValue('progressTotal');
		var currentPercent = ( (progress/total) * 100 ).toFixed(1);

		if ( document.querySelector('.progress-bar') ){
			document.querySelector('.progress-bar').style.width = currentPercent + '%';
		}
		if ( document.querySelector('.complete-line span') ){
			document.querySelector('.complete-line span').innerHTML = currentPercent + '% ' + getPageVarValue('message');
		}
	}

	/* Either run the DOM update functions once for a published page or continuously for within the builder. */
	if ( typeof window.top.App === 'undefined' ) {
		// Published page
		$(window).on('load', function(){
			updateProgressCountHTML();
			calculatePercent();
		});
	} else {
		// within the builder
		setInterval( function(){
			updateProgressCountHTML();
			calculatePercent();
		}, 500);
	}
});


/*---------------------------
 Display date
 ----------------------------*/

$(document).ready(function () {
    var mydate = new Date()
    var year = mydate.getYear()
    if (year < 1000)
        year += 1900
    var day = mydate.getDay()
    var month = mydate.getMonth()
    var daym = mydate.getDate()
    if (daym < 10)
        daym = "0" + daym
    var dayarray = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday")
    var montharray = new Array("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December")
    $("#date").append("" + dayarray[day] + ", " + montharray[month] + " " + daym + ", " + year + "");
});

/*---------------------------
 Social popup windows
 ----------------------------*/
$(document).ready(function () {
    $('.pop-twitter').click(function (event) {
        var width = 575,
            height = 400,
            left = ($(window).width() - width) / 2,
            top = ($(window).height() - height) / 2,
            url = this.href,
            opts = 'status=1' +
                ',width=' + width +
                ',height=' + height +
                ',top=' + top +
                ',left=' + left;

        window.open(url, 'twitter', opts);

        return false;
    });
    $('.pop-facebook').click(function (event) {
        var width = 575,
            height = 400,
            left = ($(window).width() - width) / 2,
            top = ($(window).height() - height) / 2,
            url = this.href,
            opts = 'status=1' +
                ',width=' + width +
                ',height=' + height +
                ',top=' + top +
                ',left=' + left;

        window.open(url, 'facebook', opts);

        return false;
    });
    $('.pop-google').click(function (event) {
        var width = 575,
            height = 400,
            left = ($(window).width() - width) / 2,
            top = ($(window).height() - height) / 2,
            url = this.href,
            opts = 'status=1' +
                ',width=' + width +
                ',height=' + height +
                ',top=' + top +
                ',left=' + left;

        window.open(url, 'google', opts);

        return false;
    });
});

// Must load 3rd party API after data-attributes have been updated.

/* Twitter */
!function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (!d.getElementById(id)) {
        js = d.createElement(s);
        js.id = id;
        js.src = "//platform.twitter.com/widgets.js";
        fjs.parentNode.insertBefore(js, fjs);
    }
}(document, "script", "twitter-wjs");
/* Google plus */
!(function () {
    var po = document.createElement('script');
    po.type = 'text/javascript';
    po.async = true;
    po.src = 'https://apis.google.com/js/plusone.js';
    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(po, s);
})();
/* Facebook */
!(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s);
    js.id = id;
    js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));






