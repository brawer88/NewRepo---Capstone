//(function($) {

//	"use strict";

//	var fullHeight = function() {

//		$('.js-fullheight').css('height', $(window).height());
//		$(window).resize(function(){
//			$('.js-fullheight').css('height', $(window).height());
//		});

//	};
//	fullHeight();

//	$('#sidebarCollapse').on('click', function () {
//      $('#sidebar').toggleClass('active');
//  });

//})(jQuery);

function showNutrition() {
	var AS = document.getElementById("nutritionDiv");
	if (AS.style.display === "none") {
		$("#btnNutrition").html("Hide Nutrition");
		AS.style.display = "block";
	} else {
		$("#btnNutrition").html("Show Nutrition");
		AS.style.display = "none";
	}
}

// will update when procedures are in place
function rateRecipe(UID, RecipeID, intDifficultyRating, intTasteRating) { // rating type is 1 for difficulty and 2 for taste
	try {
		var ajaxData = { //json structure
			UID: UID,
			RecipeID: RecipeID,
			intDifficultyRating: intDifficultyRating,
			intTasteRating: intTasteRating
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