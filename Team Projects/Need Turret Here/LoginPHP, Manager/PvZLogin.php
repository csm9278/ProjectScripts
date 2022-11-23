
	<?php
		$u_id = $_POST["Account"];
		$u_pw = $_POST["Password"];

		$con = mysqli_connect(-);			//같은위치에 있는 서버에 있는  mySQL에 접근하겟다 ->  localhost
		//localhost -> 같은 서버 내

		if(!$con)
			die("Could not Connect" . mysqli_connect_error());
		//연결 실패 했을 경우 이 스크립트를 닫아주겠다는 뜻

		$check = mysqli_query($con, "SELECT * FROM GreateTeam WHERE Account = '". $u_id ."' ");
		
		$numrows = mysqli_num_rows($check);

		if($numrows == 0)
		{
			//mysqli_num_rows() 함수는 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼 때 쓰임, 즉 0이라는 뜻은 해당못 찾았다는 뜻
			die("ID does not exist.\n");
		}
	

		$row = mysqli_fetch_assoc($check);	//user_id에 해당 되는 행의 내용을 가져온다.
		if($row)
		{
			if($u_pw == $row["Password"])
			{
				//echo $row["nick_name"] . " : " . $row["myinfo"];
				//PHP에서의 JSon 생성 코드
				$RowDatas = array();
				
				$RowDatas["SlotData1"] = $row["Sv_slot1"];
				$RowDatas["SlotData2"] = $row["Sv_slot2"];
				$RowDatas["SlotData3"] = $row["Sv_slot3"];
				$RowDatas["Diamond1"] = $row["Sv_dia1"];
				$RowDatas["Diamond2"] = $row["Sv_dia2"];
				$RowDatas["Diamond3"] = $row["Sv_dia3"];

				$output = json_encode($RowDatas,JSON_UNESCAPED_UNICODE);
				//PHP 5.4이상 JSON 형식 생성
				echo $output; 
				echo "\n";
				echo "Login-Success!!";
			}
			else
			{
				die("PassWord does not Match.\n");			
			}
		}

		mysqli_close($con);

	?>


