$(window).load(function () {
    $.plot(
        $("!placeholderName!"),
        "!data!",
        {
            grid: {
                hoverable: true,
                clickable: true
            },
            series: {
                lines: {
                    show: true
                }, points: {
                    show: true
                }
            },
            zoom: {
                interactive: true
            },
            pan: {
                interactive: true
            }
        }
    );

    $("<div id='tooltip'></div>").css({
        position: "absolute",
        border: "1px solid #fdd",
        padding: "2px",
        "background-color": "#fee",
        opacity: 0.80
    }).appendTo("body");
    const tooltip = $("#tooltip").hide();

    let oldItem = null;
    $("!placeholderName!").bind("plothover", function (event, pos, item) {
        let tooltip = $("#tooltip");
        if (item && oldItem !== item) {
            oldItem = item;
            const x = item.datapoint[0].toFixed(2),
                y = item.datapoint[1].toFixed(2);

            tooltip.html("$f\\left(" + x + "\\right) \\approx " + y + "$")
                .css({top: item.pageY + 5, left: item.pageX + 5})
                .fadeIn(200);
            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        } else if (!item && tooltip.is(":visible")) {
            tooltip.hide();
        }
    });
});