jQuery(document).ready(function () {
    jQuery("#visibile_form").on("click", function () {
        jQuery(".hidden_form").show();

    });
    jQuery(".close").on("click", function () {
        jQuery(".hidden_form").hide();

    });

    function setMargins() {
        height = $(window).height();
        containerHeight = $(".hidden_form").height();
        topMargin = (height - containerHeight) / 2;
        $(".hidden_form").css("top", topMargin);
        width = $(window).width();
        containerWidth = $(".hidden_form").width();
        leftMargin = (width - containerWidth) / 2;
        $(".hidden_form").css("marginLeft", leftMargin);
    }

    $(document).ready(function () {
        setMargins();
        $(window).resize(function () {
            setMargins();
        });
    });
});