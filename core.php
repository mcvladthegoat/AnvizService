<?php
/*CORE.PHP*/
header("Content-type: text/html; charset=utf-8");
$cfg = parse_ini_file("config.ini");
$con = odbc_connect("Driver={SQL Server Native Client 11.0};Server=".$cfg["SRV"].";Database=".$cfg["DB"].";Integrated Security=SSPI;", $cfg["USER"], $cfg["PWD"]);
function get_current($con)
{
    $users = array();
    $sensors = array('1' => '6 этаж',  '2' => '7 этаж', '3' => '3 этаж', '4' => 'Чебоксары');
	$dt = date("Y-m-d", time());
	$sqlexp = "SELECT Userinfo.userid as id, CAST(Userinfo.Name as NVARCHAR(70)) as name_converted, AnvizCheckins.s_id as m_s,
	 count(AnvizCheckins.id) as count_checkins, MAX(AnvizCheckins.time) as m_d FROM Userinfo
	  inner join AnvizCheckins ON AnvizCheckins.p_id = Userinfo.userid WHERE AnvizCheckins.time BETWEEN '$dt 04:00:00' AND '$dt 23:59:59' GROUP BY CAST(Userinfo.name as NVARCHAR(70)), UserInfo.userid, AnvizCheckins.s_id
	    ORDER BY CAST(Userinfo.name as NVARCHAR(70))";
	$res2 = odbc_exec($con, $sqlexp);
	while (odbc_fetch_row($res2)){
		$date = strtotime(iconv("windows-1251","UTF-8",odbc_result($res2, "m_d")));
		$id = iconv("windows-1251","UTF-8",odbc_result($res2, "id"));
		$sensor_id = iconv("windows-1251","UTF-8",odbc_result($res2, "m_s"));
		$checkins = iconv("windows-1251","UTF-8",odbc_result($res2, "count_checkins"));
		if(array_key_exists($id, $users)) {
			if($users[$id]["date"] < $date){
				$users[$id]["date"] = $date;
				$users[$id]["sensor"] = $sensor_id;
			}
			$users[$id]["status"] += $checkins;
		}
		else{
			$users[$id]["date"] = $date;
			$users[$id]["status"] = $checkins;
			$users[$id]["name"] = iconv("windows-1251","UTF-8",odbc_result($res2, "name_converted"));
			$users[$id]["sensor"] = $sensor_id;
		}
	}
	echo "<tr>
            <td>ФИО</td>
            <td>Последняя отметка</td>
            <td>Статус</td>
            <td>Расположение</td>
         </tr>";
         $months_rus = array('01'=>'Января', '02'=>'Февраля', '03'=>'Марта', '04'=> 'Апреля',
 '05'=>'Мая', '06'=>'Июня', '07'=>'Июля', '08'=>'Августа', '09'=>'Сентября', 
 10=>'Октября', 11=>'Ноября', 12=>'Декабря');
	foreach ($users as $key => &$value) {
		$t_status = ($value["status"] % 2 == 0) ? "<span class='label label-danger'>&nbsp</span>" :
		 "<span class='label label-success'>&nbsp</span>";
		$t_sensor = $sensors[$value["sensor"]];
		$t_name = ($value["name"] == "") ? "[Нет имени]" : $value["name"];
		$f_month = $months_rus[date('m', $value['date'])];
		$t_date = date('h:i:s', $value['date']);
		echo "<tr><td>$t_name</td><td>$t_date</td><td>$t_status</td><td>$t_sensor</td></tr>";
	}


}

function mem_usage()
{
	echo memory_get_usage(true);
}

$call_func = $_GET["act"];
if (function_exists($call_func)) $call_func($con); else die("Wrong call");

odbc_close($con);