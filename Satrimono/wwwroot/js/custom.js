
  
  /*  Start menu */
  
 $(document).ready(function() {
		$menuLeft = $('.pushmenu-left');
		$nav_list = $('#nav_list');
		
		$nav_list.click(function() {
			$(this).toggleClass('active');
			$('.pushmenu-push').toggleClass('pushmenu-push-toright');
			$menuLeft.toggleClass('pushmenu-open');
		});
    
        /*$('.pushmenu-left a').click(function(){
            $nav_list.removeClass("active");
            $('.pushmenu-push').removeClass('pushmenu-push-toright');
            $menuLeft.removeClass('pushmenu-open');
        }); */
	});
	
	
	
	
(function(e, t, n, r) {
    e.fn.doubleTapToGo = function(r) {
        if (!("ontouchstart" in t) && !navigator.msMaxTouchPoints && !navigator.userAgent.toLowerCase().match(/windows phone os 7/i)) return false;
        this.each(function() {
            var t = false;
            e(this).on("click", function(n) {
                var r = e(this);
                if (r[0] != t[0]) {
                    n.preventDefault();
                    t = r
                }
            });
            e(n).on("click touchstart MSPointerDown", function(n) {
                var r = true,
                    i = e(n.target).parents();
                for (var s = 0; s < i.length; s++)
                    if (i[s] == t[0]) r = false;
                if (r) t = false
            })
        });
        return this
    }
})(jQuery, window, document);
	



	$( function()
	{
		$( '#nav li:has(ul)' ).doubleTapToGo();
	});	
	
	
 /*  End menu */
 
 	
	
	 /*  Start dot */
	 
 
$.each( $( ".mission-vision-text" ), function() {
   // alert("");
  var p=  $(this).find("p");
    var divh=$(this).height();
    
    while ($(p).outerHeight() > divh) {
		
    
    $(p).text(function (index, text) {
        return text.replace(/\W*\s(\S)*$/, '...');
        
    });
}
    
    
});


$.each( $( ".banner-looking-box-text" ), function() {
   // alert("");
  var p=  $(this).find("p");
    var divh=$(this).height();
    
    while ($(p).outerHeight() > divh) {
		
    
    $(p).text(function (index, text) {
        return text.replace(/\W*\s(\S)*$/, '...');
        
    });
}
    
    
});


$.each( $( ".testimonial-text" ), function() {
   // alert("");
  var p=  $(this).find("p");
    var divh=$(this).height();
    
    while ($(p).outerHeight() > divh) {
		
    
    $(p).text(function (index, text) {
        return text.replace(/\W*\s(\S)*$/, '...');
        
    });
}
    
    
});


 
  
 
 
 
 
  /*  Start wow */ 

 new WOW().init();
 
  

   $(document).ready(function(){
	
	//Check to see if the window is top if not then display button
	$(window).scroll(function(){
		if ($(this).scrollTop() > 100) {
			$('.scrollToTop').fadeIn();
		} else {
			$('.scrollToTop').fadeOut();
		}
	});
	
	//Click event to scroll to top
	$('.scrollToTop').click(function(){
		$('html, body').animate({scrollTop : 0},800);
		return false;
	});
	
});
 
 
 
 
  
 function myMap() {

var mapCanvas = document.getElementById("map");
  var myCenter = new google.maps.LatLng(27.9853033, -82.4369414);
  var mapOptions = {center: myCenter, zoom: 18};
  var map = new google.maps.Map(mapCanvas,mapOptions);
  var marker = new google.maps.Marker({
    position: myCenter,
    animation: google.maps.Animation.BOUNCE
  });
  marker.setMap(map);
 
 }