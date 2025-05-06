$(function(){

	if($('body').attr('data-page') == 'login' || $('body').attr('data-page') == 'signup' || $('body').attr('data-page') == 'password'){
		
		/*  For icon rotation on input box foxus  */ 	
		$('.input-field').focus(function() {
				$('.page-icon img').addClass('rotate-icon');
		});

		/*  For icon rotation on input box blur  */ 	
		$('.input-field').blur(function() {
				$('.page-icon img').removeClass('rotate-icon');
		});
	};

	/*  Background slide for lockscreen page  */
	if($('body').attr('data-page') == 'lockscreen'){
		$.backstretch([ "~/img/background/01.png", "~/img/background/02.png", "~/img/background/03.png", "~/img/background/04.png", "~/img/background/05.png", "~/img/background/06.png",
		  "~/img/background/07.png", "~/img/background/08.png", "~/img/background/09.png" ], 
		  {
		    fade: 600,
		    duration: 4000 
		});
	}

});


