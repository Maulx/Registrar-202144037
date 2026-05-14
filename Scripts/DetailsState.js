function RestoreDetailsState() {

    $("details").off("toggle");

    $("details").on("toggle", function () {
        let details_dom = $(this)[0];

        if (details_dom != undefined) {
            localStorage.setItem(details_dom.id, details_dom.open);
        }
    });

    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);

        if (key.indexOf("details") > -1) {
            let details_dom = $("#" + key)[0];

            if (details_dom != undefined) {
                details_dom.open = localStorage.getItem(key) == "true";
            }
        }
    }
}