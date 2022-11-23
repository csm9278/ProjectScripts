<?php

	$Account = $_POST["Account"];
	$Diamonds = $_POST["Diamonds"];
	$Stage = $_POST["Stage"];

	if(empty($Account))
		die("Account is empty. \n");
	if(empty($Diamonds))
		die("Diamonds is empty. \n");
	if(empty($Stage))
		die("Stage is empty. \n");


	$con = mysqli_connect(-);

	if(!$con)
		die("Could not Connet" . mysqli_connect_error());
	//연결 실패 했을 경우 이 스크립트를 닫아주겠다는 뜻

	$check = mysqli_query($con, "SELECT Account FROM GreateTeam WHERE Account = '". $Account ."' ");

	$numrows = mysqli_num_rows($check);
	if($numrows == 0)
	{   
		die("ID does not exist. \n");
	}

	if( $row = mysqli_fetch_assoc($check) ) //Account 이름에 해당하는 행을 찾아준다.
	{
		//mysqli_query($con, "UPDATE GreateTeam SET Diamonds = '". $Diamonds ."' WHERE Account = '". $Account ."' ");
		//Account 를 찾아서 mygold = '$Diamonds' 로 변경하라는 뜻 
		mysqli_query($con, "UPDATE GreateTeam SET `Diamonds` = '".$Diamonds."' ,`Stage` = '". $Stage ."' WHERE Account = '". $Account ."' ");

		echo("UpDateSuccess~");
	}

	mysqli_close($con);
?>