$(function () {
    $('.gathererTooltip').each(function () {
        var image = $('<img src="' + $(this).data().image + '" style="display:none"></img>');
        $('body').append(image);
        $(image).css({
            position: "absolute",
            top: $(this).position().top + $(this).height(),
            left: $(this).position().left + 10
        });
        for (var prop in $(this).data()) {
            if (prop != "image") {
                $(image).css(prop, $(this).data()[prop]);
            }
        };
        $(this).hover(
            function () { $(image).fadeIn(100); },
            function () { $(image).fadeOut(100); }
        );
    });
});