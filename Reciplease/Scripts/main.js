(function($) {

	"use strict";

	var fullHeight = function() {

		$('.js-fullheight').css('height', $(window).height());
		$(window).resize(function(){
			$('.js-fullheight').css('height', $(window).height());
		});

	};
	fullHeight();

	$('#sidebarCollapse').on('click', function () {
      $('#sidebar').toggleClass('active');
  });

})(jQuery);

// will update when procedures are in place
function rateRecipe(UID, RecipeID, Rating, intDifficultyRating, intTasteRating) { // rating type is 1 for difficulty and 2 for taste
	try {
		var ajaxData = { //json structure
			UID: uid,
			ID: id,
			Rating: rating
		}

		$.ajax({
			type: "POST",
			url: "../../Home/RateRecipe",
			data: ajaxData,
			success: function (returnData) {
				var i;

				//remove all "checked" stars
				for (i = 1; i <= 5; i++) {
					$("#rate".concat(i)).removeClass("checked");
				}

				//add updated "checked" stars
				for (i = 1; i <= rating; i++) {
					$("#rate".concat(i)).addClass("checked");
				}
			},
			error: function (xhr) {
				debugger;
			}
		});
	}
	catch (err) {
		showError(err);
	}
}