<?php
 header('Content-type: text/html; charset=utf-8');
/* With this PHP script, we are downloading information from this webpage:
http://www.zodynas.lt/terminu-zodynas .
This website holds about 300 K words in lithuanian.
Due to download limitations from the website, we are downloading only 200 pages per script launch,
thus we need to adjust indexes each time.
.*/

// Open a .txt file to store the data.
$myfile = fopen("visi_zodziai.txt", "a");

$url  = "http://www.zodynas.lt/terminu-zodynas/?page=1";
$data = file_get_contents($url);
echo $data;
// We are downloading pure HTML code to check whetver the website didn't block us.

// Main loop.
for ( $x = 1921;$x <= 1921;$x++)
{
  // Download data from the url.
  $url  = "http://www.zodynas.lt/terminu-zodynas/?page=$x";
  $data = file_get_contents($url);

  $dom = new DOMDocument();
  libxml_use_internal_errors(true);
  $dom->loadHTML($data);
  libxml_use_internal_errors(false);
  // For each a ( url ) element in the HTML.
  foreach($dom->getElementsByTagName('a') as $a) 
  {
    // Get each elements class to determine whetver the element holds the word.
    $a_class = $a->getAttribute("class");    
    if ( strlen($a->nodeValue) > 0 && $a_class == "col-xs-6 col-sm-2 no-break o-h")
    {
      $a->nodeValue = $a->nodeValue . "\n";
      fwrite($myfile,$a->nodeValue);
    }
  }
}
// Close the file.
fclose($myfile);
?>