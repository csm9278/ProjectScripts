<?php

	$Account = $_POST["Account"];
	$Diamonds = $_POST["Diamonds"];
	$Itemlist = $_POST["Inventory"];

	if(empty($Account))
		die("Account is empty. \n");

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

	if( $row = mysqli_fetch_assoc($check) ) //user 이름에 해당하는 행을 찾아준다.
	{
		mysqli_query($con, "UPDATE GreateTeam SET `Inventory` = '". $Itemlist ."' WHERE Account = '". $Account ."' ");


		echo("BuySuccess~");
	}

	mysqli_close($con);
?>