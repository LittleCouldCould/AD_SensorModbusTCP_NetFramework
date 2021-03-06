﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace AD_Sensor_SM9001A
{
    class SQLCreate
    {
        Create_Init create_Init = new Create_Init();
        Create_Table create_Table = new Create_Table();
        SQLInsert sqlInsert = new SQLInsert();
        Stopwatch stopwatch = new Stopwatch();
        public SQLCreate(BackgroundWorker worker)
        {
            try
            {
                int sensorSum = Convert.ToInt32(ConfigurationManager.AppSettings["sensorSum"]);//读取配置文件
                int nodeSum = Convert.ToInt32(ConfigurationManager.AppSettings["nodeSum"]);
                //创建数据库
                worker.ReportProgress(10, "创建数据库" + ConfigurationManager.AppSettings["database"].ToString() + "");
                if (!create_Init.Create_DB(ConfigurationManager.AppSettings["database"].ToString()))
                {
                    throw new Exception("创建数据库失败");
                }

                //创建用户并授权（对应的数据库的最小权限）
                worker.ReportProgress(15, "检查用户");
                if (!create_Init.Create_User("adsensor", "adsensor"))
                {
                    throw new Exception("创建用户失败");
                }

                worker.ReportProgress(20, "创建数据表pwd");
                if (!create_Table.IsTable_Exit("pwd"))
                {
                    if (!create_Table.CreateTable("pwd", "create table pwd(pwdId tinyint unsigned, userName varchar(20) ,passWord varchar(32),primary key(pwdId))"))
                    {
                        throw new Exception("创建数据表Pwd失败");
                    }
                    else
                    {
                        if (!sqlInsert.Insert("insert into pwd(pwdId, userName, passWord) values(1, 'generalUser', '" + Encryption.GetMD5Hash("userSM9001") + "')"))
                        {
                            throw new Exception("插入数据表Pwd失败");
                        }
                        if (!sqlInsert.Insert("insert into pwd(pwdId,userName,passWord) values (2,'sysOperator','" + Encryption.GetMD5Hash("operatorSM9001") + "')"))
                        {
                            throw new Exception("插入数据表Pwd失败");
                        }
                        if (!sqlInsert.Insert("insert into pwd(pwdId,userName,passWord) values (3,'sysAdministrator','" + Encryption.GetMD5Hash("adminSM9001") + "')"))
                        {
                            throw new Exception("插入数据表Pwd失败");
                        }
                    }
                }


                worker.ReportProgress(30, "创建数据表prjname");
                if (!create_Table.IsTable_Exit("prjname"))
                {
                    if (!create_Table.CreateTable("prjname", "create table prjname(prjId  tinyint unsigned,prjName varchar(32),primary key(prjId))"))
                    {
                        throw new Exception("创建数据表prjname失败");
                    }
                    else
                    {
                        if (!sqlInsert.Insert("insert into prjname(prjId,prjName) values ('1','无锡圣敏传感科技股份有限公司')"))
                        {
                            throw new Exception("插入数据表pejname失败");
                        }
                    }
                }

                worker.ReportProgress(30, "创建数据表sensor");
                if (!create_Table.IsTable_Exit("sensor"))
                {
                    if (!create_Table.CreateTable("sensor", "create table sensor(sensorId smallint,sensorName varchar(20),startNo smallint,quantity smallint,length smallint ,able tinyint(1),locationx double,locationy double ,areaid smallint,display tinyint(1),primary key(sensorId))"))
                    {
                        throw new Exception("创建数据表sensor失败");
                    }
                    else
                    {
                        for (int i = 1; i <= sensorSum; i++)
                        {
                            if (!sqlInsert.Insert("insert into sensor(sensorId,sensorName,startNo,quantity,length,able,locationx,locationy,areaid,display) values ('" + i + "','探测器" + i + "','1'," + nodeSum + ",'5',0,0,0,0,0)"))
                            {
                                throw new Exception("插入数据表sensor失败");
                            }
                        }
                    }
                }
                worker.ReportProgress(40, "创建数据表node");
                if (!create_Table.IsTable_Exit("node"))
                {
                    if (!create_Table.CreateTable("node", "create table node(sensorId smallint,nodeId smallint,nodeName varchar(20),able tinyint(1),locationx double,locationy double ,areaid smallint,display tinyint(1),primary key(sensorId,nodeId))"))
                    {
                        throw new Exception("创建数据表node失败");
                    }
                    else
                    {
                        //stopwatch.Start();
                        for (int i = 1; i <= sensorSum; i++)
                        {
                            for (int j = 1; j <= nodeSum; j++)
                            {
                                if (!sqlInsert.Insert("insert into node(sensorId,nodeId,nodeName,able,locationx,locationy,areaid,display) values ('" + i + "','" + j + "','" + j * 5 + "米',1,0,0,0,0) "))
                                {
                                    throw new Exception("插入数据表node失败");
                                }
                            }
                        }

                        //if (!new SQLProcedure().Call_Procedure("insert_node"))
                        //{
                        //    throw new Exception("插入数据表node失败");
                        //}
                        //stopwatch.Stop();
                        //string time = stopwatch.ElapsedMilliseconds.ToString();
                    }
                }
                worker.ReportProgress(50, "创建数据表area");
                if (!create_Table.IsTable_Exit("area"))
                {
                    if (!create_Table.CreateTable("area", "create table area(areaId smallint,orderId smallint,areaName varchar(20),able tinyint(1),primary key(areaId))"))
                    {
                        throw new Exception("创建数据表area失败");
                    }
                    else
                    {
                        for (int i = 1; i <= 50; i++)
                        {
                            if (!sqlInsert.Insert("insert into area(areaId,orderId,areaName,able) values ('" + i + "','" + i + "','分区" + i + "',0)"))
                            {
                                throw new Exception("插入数据表area失败");
                            }
                        }
                    }
                }
                worker.ReportProgress(60, "创建数据表temper");
                if (!create_Table.IsTable_Exit("temper"))
                {
                    if (!create_Table.CreateTable("temper", "create table temper(sensorId smallint,nodeId smallint,dateTime datetime ,status varchar(5),primary key(sensorId,nodeId))"))
                    {
                        throw new Exception("创建数据表temper失败");
                    }
                }
                worker.ReportProgress(70, "创建数据表alarm");
                if (!create_Table.IsTable_Exit("alarm"))
                {
                    if (!create_Table.CreateTable("alarm", "create table alarm(sensorId smallint,nodeId smallint,dateTime datetime ,status varchar(5),primary key(sensorId,nodeId))"))
                    {
                        throw new Exception("创建数据表alarm失败");
                    }
                }
                //if (!IsTabExit("pagepic"))
                //{
                //    worker.ReportProgress(70, "创建数据表pagepic");
                //    if (!Create_Table_PagePic())
                //    {
                //        throw new Exception("创建数据表pagePic失败");
                //    }
                //}
                //if (!IsTabExit("alarm"))
                //{
                //    worker.ReportProgress(80, "创建数据表alarm");
                //    if (!Create_Table_alarm())
                //    {
                //        throw new Exception("创建数据表alarm失败");
                //    }
                //}
                //if (!IsTabExit("audit"))
                //{
                //    worker.ReportProgress(90, "创建数据表audit");
                //    if (!Create_Table_audit())
                //    {
                //        throw new Exception("创建数据表audit失败");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log.log.Error("数据库创建失败！", ex);
                MessageBox.Show("数据库创建失败" + ex.Message);
            }
            finally
            {
                worker.CancelAsync();
            }
        }
    }
}
