<?php

	$u_id = $_POST["Account"];
	$u_pw = $_POST["Password"];
	$nick = $_POST["NickName"];

	if(empty($u_id))
		die("u_acc is empty. \n");
	if(empty($u_pw))
		die("u_pw is empty. \n");
	if(empty($nick))
		die("nick is empty. \n");

	$con = mysqli_connect(-);

	if(!$con)
		die("Could not connect to server." . mysqli_connect_error());
		

	$check = mysqli_query($con, "SELECT Account FROM GreateTeam WHERE Account = '".$u_id."' ");
	$numrows = mysqli_num_rows($check);	
	
	if($numrows != 0)
	{	
		die("ID does exist.\n");		
	}

	$check = mysqli_query($con, "SELECT NickName FROM GreateTeam WHERE NickName = '".$nick."' ");
	$numrows = mysqli_num_rows($check);	
	if($numrows != 0)
	{	
		die("NickName does exist.\n");
	}

	$Result = mysqli_query($con,			
	"INSERT INTO GreateTeam (Account, Password, NickName)
	VALUES ('" . $u_id . "', '" . $u_pw . "', '" . $nick . "');");

	
	if($Result)
		echo "Create Success. \n";
	else
		echo "Create error. \n";

	mysqli_close($con);
			
?>