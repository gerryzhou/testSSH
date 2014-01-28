function popUp(URL,STATUS) {
day = new Date();
id = day.getTime();
eval("page" + id + " = window.open(URL, '" + id + "', STATUS);");
}

function closeIt() {
 close();
}

ns4 = (document.layers)? true:false
ie4 = (document.all)? true:false
ns6 = (document.getElementById)? true:false

// Show/Hide functions for non-pointer layer/objects

function show(id) {
	if (ns4) document.layers[id].visibility = "show"
	else if (ie4) document.all[id].style.visibility = "visible"
	else if (ns6) document.getElementById(id).style.visibility = "visible"
	
}

function hide(id) {
	if (ns4) document.layers[id].visibility = "hide"
	else if (ie4) document.all[id].style.visibility = "hidden"
	else if (ns6) document.getElementById(id).style.visibility = "hidden"
}


function newImage(arg) {
	if (document.images) {
		rslt = new Image();
		rslt.src = arg;
		return rslt;
	}
}

function changeImages() {
	if (document.images && (preloadFlag == true)) {
		for (var i=0; i<changeImages.arguments.length; i+=2) {
			document[changeImages.arguments[i]].src = changeImages.arguments[i+1];
		}
	}
}

var preloadFlag = false;
function preloadImages() {
	if (document.images) {
		preloadFlag = true;
	}
}



function MM_swapImgRestore() { //v3.0
  var i,x,a=document.MM_sr; for(i=0;a&&i<a.length&&(x=a[i])&&x.oSrc;i++) x.src=x.oSrc;
}

function MM_preloadImages() { //v3.0
  var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
    var i,j=d.MM_p.length,a=MM_preloadImages.arguments; for(i=0; i<a.length; i++)
    if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
}

function MM_findObj(n, d) { //v4.01
  var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
    d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
  if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
  for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document);
  if(!x && d.getElementById) x=d.getElementById(n); return x;
}

function MM_swapImage() { //v3.0
  var i,j=0,x,a=MM_swapImage.arguments; document.MM_sr=new Array; for(i=0;i<(a.length-2);i+=3)
   if ((x=MM_findObj(a[i]))!=null){document.MM_sr[j++]=x; if(!x.oSrc) x.oSrc=x.src; x.src=a[i+2];}
}


function MM_reloadPage(init) {  //reloads the window if Nav4 resized
  if (init==true) with (navigator) {if ((appName=="Netscape")&&(parseInt(appVersion)==4)) {
    document.MM_pgW=innerWidth; document.MM_pgH=innerHeight; onresize=MM_reloadPage; }}
  else if (innerWidth!=document.MM_pgW || innerHeight!=document.MM_pgH) location.reload();
}
MM_reloadPage(true);

function MM_showHideLayers() { //v6.0
  var i,p,v,obj,args=MM_showHideLayers.arguments;
  for (i=0; i<(args.length-2); i+=3) if ((obj=MM_findObj(args[i]))!=null) { v=args[i+2];
    if (obj.style) { obj=obj.style; v=(v=='show')?'visible':(v=='hide')?'hidden':v; }
    obj.visibility=v; }
}

function MM_openBrWindow(theURL,winName,features) { //v2.0
  window.open(theURL,winName,features);
}


var today = new Date(); // today
var year = y2k(today.getYear());
function dateMsg() {
	return(year);
}
function y2k(year) {
	if (year < 2000)
	year = year + 1900;
	return year;
}


function get_primary_nav() {
  var urlString = location.href.toLowerCase();
  site1 = urlString.lastIndexOf("12345");
  site12 = urlString.lastIndexOf("com/index");
  site2 = urlString.lastIndexOf("educationcenter");
  site3 = urlString.lastIndexOf("/traders/");
  site4 = urlString.lastIndexOf("/investors");
  site5 = urlString.lastIndexOf("hedgefunds");
  site6 = urlString.lastIndexOf("/proptrader");
  site7 = urlString.lastIndexOf("international");
  site8 = urlString.lastIndexOf("otherservices");
  site9 = urlString.lastIndexOf("/company/");
  site10 = urlString.lastIndexOf("buy");
  site11 = urlString.lastIndexOf("/login/");
  site13 = urlString.lastIndexOf("gttforum");
  site14 = urlString.lastIndexOf("livesupport");
  site15 = urlString.lastIndexOf("/misc");
  site16 = urlString.lastIndexOf("/cgi");
  site17 = urlString.lastIndexOf("/sitemap");
  site18 = urlString.lastIndexOf("/disclaimer");
  site19 = urlString.lastIndexOf("/privacy");
  site20 = urlString.lastIndexOf("/sfomagazine");
  site21 = urlString.lastIndexOf("/2003");
  site22 = urlString.lastIndexOf("/2004");
  site23 = urlString.lastIndexOf("/activetradermag");
  site24 = urlString.lastIndexOf("/entities");
  site25 = urlString.lastIndexOf("/affiliate");
  site26 = urlString.lastIndexOf("/client");
  site27 = urlString.lastIndexOf("/gtttradeloglogin");
  site28 = urlString.lastIndexOf("/professionals/");
  site29 = urlString.lastIndexOf("/external");
  site30 = urlString.lastIndexOf("/law/");
  site31 = urlString.lastIndexOf("/manuscripts/");
  site32 = urlString.lastIndexOf("com/law/index");
  site33 = urlString.lastIndexOf("/contactus/");
  site34 = urlString.lastIndexOf("/traders/advocacy");
  site35 = urlString.lastIndexOf("/hedgefunds/hedge");
  site36 = urlString.lastIndexOf("/hedgefunds/off");
  site37 = urlString.lastIndexOf("/hedgefunds/reg");
  site38 = urlString.lastIndexOf("/hedgefunds/start");
  site39 = urlString.lastIndexOf("/hedgefunds/value");
  site40 = urlString.lastIndexOf("/hedgefunds/legal");
  site41 = urlString.lastIndexOf("/hedgefunds/complianceprogramdesign");
  site42 = urlString.lastIndexOf("/hedgefunds/incubator");
  site43 = urlString.lastIndexOf("/tools/");
  site44 = urlString.lastIndexOf("/hedgefunds/index");
  site45 = urlString.lastIndexOf("/blog/");
  site46 = urlString.lastIndexOf("/purchases/");
  site47 = urlString.lastIndexOf("/activeinvestors");
  site48 = urlString.lastIndexOf("/association");
  site49 = urlString.lastIndexOf("/greenstradertaxguide");
  site50 = urlString.lastIndexOf("/gtttradersassociation");
  site51 = urlString.lastIndexOf("/professionals-trader");
  site52 = urlString.lastIndexOf("/professionals-investment");
  site53 = urlString.lastIndexOf("/professionals-general");
  site54 = urlString.lastIndexOf("/quick-links");
  site55 = urlString.lastIndexOf(".com");
  site56 = urlString.lastIndexOf("/greennfhllc");
  site57 = urlString.lastIndexOf("/proptraders");
  site58 = urlString.lastIndexOf("/newsletter");
  site59 = urlString.lastIndexOf("/investmenttax");
  site60 = urlString.lastIndexOf("/gtt-investor");

  siteName="12345";
  maxNum=site1;
  
  if (site2 > maxNum) {siteName="educationcenter"; maxNum=site2;}

  if (site35 > maxNum) {siteName="/hedgefunds/hedge"; maxNum=site35;}
  if (site36 > maxNum) {siteName="/hedgefunds/off"; maxNum=site36;}
  if (site37 > maxNum) {siteName="/hedgefunds/reg"; maxNum=site37;}
  if (site38 > maxNum) {siteName="/hedgefunds/start"; maxNum=site38;}
  if (site39 > maxNum) {siteName="/hedgefunds/value"; maxNum=site39;}
  if (site40 > maxNum) {siteName="/hedgefunds/legal"; maxNum=site40;}
  if (site41 > maxNum) {siteName="/hedgefunds/complianceprogramdesign"; maxNum=site41;}
  if (site42 > maxNum) {siteName="/hedgefunds/incubator"; maxNum=site42;}
  if (site43 > maxNum) {siteName="/tools/"; maxNum=site43;}
  if (site44 > maxNum) {siteName="/hedgefunds/index"; maxNum=site44;}
  if (site45 > maxNum) {siteName="/blog/"; maxNum=site45;}
  if (site46 > maxNum) {siteName="/purchases/"; maxNum=site46;}
  if (site4 > maxNum) {siteName="/investors"; maxNum=site4;}
  if (site5 > maxNum) {siteName="hedgefunds"; maxNum=site5;}
  if (site6 > maxNum) {siteName="/proptrader"; maxNum=site6;}
  if (site7 > maxNum) {siteName="international"; maxNum=site7;}
  if (site8 > maxNum) {siteName="otherservices"; maxNum=site8;}
  if (site9 > maxNum) {siteName="/company/"; maxNum=site9;}
  if (site10 > maxNum) {siteName="buy"; maxNum=site10;}
  if (site11 > maxNum) {siteName="/login/"; maxNum=site11;}
  if (site12 > maxNum) {siteName="com/index"; maxNum=site12;}
  if (site13 > maxNum) {siteName="gttforum"; maxNum=site13;}
  if (site14 > maxNum) {siteName="livesupport"; maxNum=site14;}
  if (site15 > maxNum) {siteName="/misc"; maxNum=site15;}
  if (site16 > maxNum) {siteName="/cgi"; maxNum=site16;}
  if (site17 > maxNum) {siteName="/sitemap"; maxNum=site17;}
  if (site18 > maxNum) {siteName="/disclaimer"; maxNum=site18;}
  if (site19 > maxNum) {siteName="/privacy"; maxNum=site19;}
  if (site20 > maxNum) {siteName="/sfomagazine"; maxNum=site20;}
  if (site21 > maxNum) {siteName="/2003"; maxNum=site21;}
  if (site22 > maxNum) {siteName="/2004"; maxNum=site22;}
  if (site23 > maxNum) {siteName="/activetradermag"; maxNum=site23;}
  if (site24 > maxNum) {siteName="/entities"; maxNum=site24;}
  if (site25 > maxNum) {siteName="/affiliate"; maxNum=site25;}
  if (site26 > maxNum) {siteName="/client"; maxNum=site26;}
  if (site27 > maxNum) {siteName="/gtttradeloglogin"; maxNum=site27;}
  if (site28 > maxNum) {siteName="/professionals/"; maxNum=site28;}
  if (site29 > maxNum) {siteName="/external"; maxNum=site29;}
  if (site30 > maxNum) {siteName="/law/"; maxNum=site30;}
  if (site31 > maxNum) {siteName="/manuscripts/"; maxNum=site31;}
  if (site32 > maxNum) {siteName="com/law/index"; maxNum=site32;}
  if (site33 > maxNum) {siteName="/contactus/"; maxNum=site33;}
  if (site34 > maxNum) {siteName="/traders/advocacy"; maxNum=site34;}
  if (site3 > maxNum) {siteName="/traders/"; maxNum=site3;}
 if (site47 > maxNum) {siteName="/activeinvestors"; maxNum=site47;}
 if (site48 > maxNum) {siteName="/association"; maxNum=site48;}
 if (site49 > maxNum) {siteName="/greenstradertaxguide"; maxNum=site49;}
 if (site50 > maxNum) {siteName="/gtttradersassociation"; maxNum=site50;}
 if (site51 > maxNum) {siteName="/professionals-trader"; maxNum=site51;}
 if (site52 > maxNum) {siteName="/professionals-investment"; maxNum=site52;}
 if (site53 > maxNum) {siteName="/professionals-general"; maxNum=site53;}
 if (site54 > maxNum) {siteName="/quick-links"; maxNum=site54;}
 if (site55 > maxNum) {siteName=".com"; maxNum=site55;}
 if (site56 > maxNum) {siteName="/greennfhllc"; maxNum=site56;}
 if (site57 > maxNum) {siteName="/proptraders"; maxNum=site57;}
  if (site58 > maxNum) {siteName="/newsletter"; maxNum=site58;}
  if (site59 > maxNum) {siteName="/investmenttax"; maxNum=site59;}
  if (site60 > maxNum) {siteName="/gtt-investor"; maxNum=site60;}

  return siteName;
}



function site_get_primary_nav() { 
	document.write("<SCRIPT>get_definitions();</SCRIPT>");

	document.write("<table width='955' border='0' cellspacing='0' cellpadding='0'>");
		document.write("<tr>");
			document.write("<td valign=top width=780 >");
			
		document.write('<div style="height:65px; width:780px; position:relative;">');
		document.write('	<div style="position:absolute; top:15px; left:10px;">'+gttimage+gtfimage+ginvimage+'</a></div>');
		document.write('	<div style="position:absolute; top:0; right:10px;">');
		
		document.write('<div class="header-copy">');
		document.write('	<form name="ccoptin" action="http://ui.constantcontact.com/d.jsp" target="_blank" method="post">');
		document.write('<a href="/">home</a> | ');
		document.write('<a href="/Login">login</a> | ');
		document.write('<a href="/Buy">pay invoice</a> | ');
		document.write('<a href="/ContactUs">contact us</a> &nbsp; ');
		document.write('<a href="http://www.facebook.com/Robert.Green.TraderTax" target=new><img src="/images/facebook.png" border="0" hspace=3></a><a href="http://twitter.com/GreenTraderTax" target=new><img src="/images/twitter.png" border="0"></a><a href="http://www.linkedin.com/in/robertagreencpa" target=new><img src="/images/linkedin.png" border="0" hspace=3></a><!--<a href="http://greentradertax.tumblr.com" target=new><img src="/images/tumblr.png" border="0"></a>-->');
		document.write('</div>');
		
		document.write('	<input type="hidden" name="m" value="1101665745035">');
		document.write('	<input type="hidden" name="p" value="oi">');
		document.write('	<table border="0" cellspacing="0" cellpadding="0" class="header-table-border">');
		document.write('		<tr><td colspan=3 height=5></td></tr><tr>');
		document.write('			<td class="email-small">Join our Email List! &nbsp; </td>');
		document.write('			<td><input name="ea" type="text" size="10" class="formtextfield">&nbsp;</td>');
		document.write('			<td><INPUT TYPE="image" SRC="/images/but_arrow.gif" WIDTH="15" HEIGHT="15" BORDER="0">&nbsp;</td>');
		document.write('		</tr>');
		document.write('	</table>');
		document.write('	</FORM>');
		document.write('	</div>');
		document.write('</div>');
		
			document.write("<img src='/images/"+headerimage+".jpg' width='780' height='"+headerheight+"' border='0' usemap='#map_header_sub'><br>");
				document.write("<table cellpadding=0 cellspacing=0 border=0 width=780 class='navsearch'>");
					document.write("<tr>");
						document.write("<td >");
  
  							document.write("<div class='nav-main"+src2+"'><a target='_top' href='/EducationCenter/index.shtml' >education</a></div>");
  							document.write("<div class='nav-main"+src3+"'><a target='_top' href='/Traders/index.shtml'  >traders</a></div>");
  							document.write("<div class='nav-main"+src60+"'><a target='_top' href='/GTT-Investor/index.shtml' >investors</a></div>");
  							document.write("<div class='nav-main"+src5+"'><a target='_top' href='/HedgeFunds/index.shtml' >investment management</a></div>");
  							document.write("<div class='nav-main"+src9+"'><a target='_top' href='/Company/index.shtml'  >about us</a></div>");
  							document.write("<div class='nav-main'><a target='_top' href='/blog/index.php'  >blog</a></div>");
  							document.write("<div class='nav-main"+src10+"'><a target='_top' href='/Buy'  >store</a></div>");
  							document.write("<div class='nav-main"+src11+"'><a target='_top' href='/newsletter.shtml'  >tools</a></div>");

  							document.write("<span id='taxstrategylayer' style='position:absolute; left:0px; top: "+layertop+"; z-index:1; visibility: "+show2+";'>");
  								//document.write("<div class='nav-main-sub'><a href='/gttforum/ubbthreads.php?Cat=' >message board</a></div>");
   								document.write("<div class='nav-main-sub'><a href='/EducationCenter/InteractiveSeminars.shtml'   >webinars</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/EducationCenter/YouTube.shtml'   >youtube</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/EducationCenter/Videos.shtml'   >videos</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/EducationCenter/InteractiveRadio.shtml'   >radio</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/blog/index.php'   >blog</a></div>");
 								document.write("<div class='nav-main-sub'><a href='/EducationCenter/InteractiveOnlineMeetings.shtml'   >podcasts</a></div>");
  								document.write("<div class='nav-main-sub'><a href' href='/EducationCenter/InteractiveTradeShows.shtml'   >trade shows</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/EducationCenter/InTheMedia.shtml'   >in the media</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Traders/Guides.shtml'   >trader tax guides</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/EducationCenter/Trader-Tax-Center/index.shtml'   >trader tax center</a></div>");
  							document.write("</span>");
  							
  							document.write("<span id='traderslayer' style='position:absolute; left:0px; top: "+layertop+"; z-index:1; visibility: "+show3+";'>");
  								document.write("<div class='nav-main-sub'><a href='/Traders/index.shtml' >overview</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Traders/TraderServices.shtml'	 >business traders</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/PropTraders/Traders.shtml'	 >prop traders</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Investors/index.shtml' >active investors</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Traders/Guides.shtml' >self-help guides & book</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Traders/Software.shtml' >self-help trade accounting software</a></div>");
  							document.write("</span>");
  							
  							document.write("<span id='hedgefundslayer' style='position:absolute; left:0px; top: "+layertop+"; z-index:1; visibility: "+show5+";'>");
  								document.write("<div class='nav-main-sub'><a href='/HedgeFunds/index.shtml'	 >overview</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/HedgeFunds/TaxPreparation.shtml' >tax compliance</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/HedgeFunds/Assurance.shtml' >assurance (audit/attest)</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/HedgeFunds/HedgeFundDevelopmentServices.shtml' >start a hedge fund</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/HedgeFunds/HedgeFundsConsulting.shtml' >consulting & payments</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/HedgeFunds/AccountingTax.shtml'	 >accounting</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/HedgeFunds/OperationsCompliance.shtml' >compliance</a></div>");
  							document.write("</span>");
  							  							
  							document.write("<span id='companylayer' style='position:absolute; left:0px; top: "+layertop+"; z-index:1; visibility: "+show9+";'>");
  								document.write("<div class='nav-main-sub'><a href='/Company/index.shtml' >overview</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Company/Process.shtml' >process</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Company/Professionals.shtml' >professionals</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Company/Testimonials.shtml' >testimonials</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/EducationCenter/InTheMedia.shtml' >media coverage</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Company/Advertising.shtml' >advertising</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Company/Careers.shtml' >careers</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/Company/GreenNFHLLC.shtml' >announcements</a></div>");
  								document.write("<div class='nav-main-sub'><a href='/ActiveInvestors/index.shtml' >active investors</a></div>");
  							document.write("</span>");
  							

  						document.write("</td>");
  						document.write("<td align='right' class='navsearch'>");
  							document.write("<table border='0' cellspacing='0' cellpadding='0'><form method='post' action='/cgi/search/search.cgi'>");
  								document.write("<tr valign='middle'>");
  									document.write("<td nowrap class='navsearch'><input type='hidden' name='index' value='index'><input name='KEYWORDS' type='text' class='formtextfield' style='font-size:10px;' size='7' value='Search' onFocus=\"if(this.value=='Search')this.value='';\"></td>");
  									document.write("<td width=5><img src='/images/pixel.gif' width=5 height=5 border=0></td>");
  									document.write("<td nowrap class='navsearch'><input type='image' src='/images/but_arrow_nav.gif' width='15' height='15' border=0 name='image'></td>");
  									document.write("<td width=5><img src='/images/pixel.gif' width=5 height=5 border=0></td>");
  								document.write("</tr></form>");
  							document.write("</table>");
  						document.write("</td>");
  					document.write("</tr>");
  				document.write("</table><br>");
   				document.write("<a href='#'><img src='/images/pixel.gif' width='780' height='20' border='0' usemap='#map_bottom_off'></a><br><div style='margin-right:0px;'>");
  				document.write("<map name='map_header_sub' id='map_header_sub'>");
  				document.write("<area shape='rect' coords='0,0,193,51' target='_top' href='/index.shtml'>");
  				document.write("</map>");
  	
}

function site_get_blank_nav() { 
    document.write("<table width='780' border='0' cellspacing='0' cellpadding='0'><tr><td colspan='2' class=copy>");
}


function frametest() {
	if (window.name == "gttxop") { document.write("<SCRIPT>site_get_blank_nav();</SCRIPT>"); }
	else  { document.write("<SCRIPT>site_get_primary_nav();</SCRIPT>"); }
}


function get_definitions() { 

  siteName=get_primary_nav();

  show1="hidden";
  show2="hidden";
  show3="hidden";
  show4="hidden";
  show5="hidden";
  show6="hidden";
  show7="hidden";
  show8="hidden";
  show9="hidden";
  show10="hidden";
  show11="hidden";
  show12="hidden";
  show43="hidden";
  show57="hidden";
  show59="hidden";
  
  showit1="hide";
  showit2="hide";
  showit3="hide";
  showit4="hide";
  showit5="hide";
  showit6="hide";
  showit7="hide";
  showit8="hide";
  showit9="hide";
  showit10="hide";
  showit11="hide";
  showit12="hide";
  showit57="hide";
  showit59="hide";

  src1="";
  src2="";
  src3="";
  src4="";
  src5="";
  src6="";
  src7="";
  src8="";
  src9="";
  src10="";
  src11="";
  src12="";
  src57="";
  src59="";
  src60="";

  src48="";
  headerimage="bk-sub-110803";
  layertop="138px";
  headerheight="51px";
  gtfimage=""; 
  gttimage="<img src='/images/logo-gtt-120214.jpg' border=0>"; 
  ginvimage=""; 
  
  if ((siteName == "com/index") || (siteName == ".com")) 			{gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=15>"; src1=""; show1="hidden"; showit1="hide"; layertop="317px"; headerheight="230px"; headerimage="bk-home-110803";}
  if ((siteName == "educationcenter") || (siteName == "gttforum") || (siteName == "livesupport") || (siteName == "/traders/advocacy") || (siteName == "/tools/"))    {src2="-on"; show2="visible"; showit2="show";  }
  if ((siteName == "/traders/") || (siteName == "/professionals-trader") || (siteName == "/investors")) {src3="-on"; show3="visible"; showit3="show";  }
  if (siteName == "/gtt-investor") {src60="-on"; gttimage=""; ginvimage="<img src='/images/logo-investtax-130911.jpg' border=0>"; }
  if ((siteName == "/proptraders/") || (siteName == "/proptrader")) {src57="-on"; show57="visible"; showit57="show";  }
  if ((siteName == "hedgefunds") || (siteName == "/professionals-investment"))	{src5="-on"; show5="visible"; showit5="show"; gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=20>"; }
  if (siteName == "/association")	{src48="-on";  }
  if (siteName == "buy")	{src10="-on";  }
  if ((siteName == "/law/") || (siteName == "international"))	{src12="-on"; show12="visible"; showit12="show";  }
  if (siteName == "otherservices") 		{src8="-on"; show8="visible"; showit8="show";  }
  if ((siteName == "/activeinvestors") 	|| (siteName == "/company/"))	{src9="-on"; show9="visible"; showit9="show";   gtfimage="<img src='/images/logo-110803-gtf.jpg' border=0>"; }
  if ((siteName == "/login/") || (siteName == "/sitemap") || (siteName == "/quick-links") || (siteName == "/purchases/") || (siteName == "/blog/")) 	{  gtfimage="<img src='/images/logo-110803-gtf.jpg' border=0 hspace=20>"; }

  if ((siteName == "/greennfhllc"))	{src9="-on"; show9="visible"; showit9="show";   gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=15>"; }
  if ((siteName == "/contactus/"))	{ gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=15>"; }
  if ((siteName == "/company/"))	{ gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=15>"; }
  if ((siteName == "/traders/"))	{ gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=15>"; }
  if ((siteName == "buy"))	{ gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=15>"; }
  if ((siteName == "/newsletter"))	{ gtfimage="<img src='/images/logo-nfh-120214.jpg' border=0 hspace=15>"; src11="-on"; }
  if ((siteName == "/investmenttax"))	{ gttimage="<img src='/images/investmenttax-us.jpg' border=0><img src='/images/logo-gtt-120214.jpg' border=0>"; }

}


function get_footer() {
	document.write("<SCRIPT>get_definitions();</SCRIPT>");
}