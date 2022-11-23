<?php

	$Account = $_POST["Account"];
	$Password = $_POST["Password"];

	if(empty($Account))
		die("u_id is empty. \n");
	if(empty($Password))
		die("u_pw is empty. \n");


	$con = mysqli_connect(-);

	if(!$con)
		die("Could not Connect". mysqli_connect_error());	//연결 실패 했을 경우 이 스크립트를 닫아주겠다는 뜻.

	$check = mysqli_query($con, "SELECT Account FROM GreateTeam WHERE Account = '". $Account . "' ");
	$numrows = mysqli_num_rows($check);
	
	if($numrows != 0)
	{	//즉 0이 아니라는 뜻은 내가 생성하려고 하는 아이디 값이 존재한다는 뜻.
		// 누군가 이미 사용하고 있다는 뜻
		die("ID does exist. \n");
	}

	$Result = mysqli_query($con,
	"INSERT INTO GreateTeam (Account, Password) VALUES
	('". $Account ."', '". $Password ."' );");

	if($Result)
		echo "Create Success. \n";
	else
		echo "Create error. \n";

	mysqli_close($con);

?>