/*
	TUIO C# Demo - part of the reacTIVision project
	Copyright (c) 2005-2014 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TUIO;
using TUIO_TEST;
using System.Speech.Synthesis;
using OSC;
using OSC.NET;

public class TuioDemo : Form , TuioListener
	{



	Form1 obj = null;
		private TuioClient client;

	OSCTransmitter transmitter;

	private Dictionary<long,TuioDemoObject> objectList;
		private Dictionary<long,TuioCursor> cursorList;
		private Dictionary<long,TuioBlob> blobList;
		private object cursorSync = new object();
		private object objectSync = new object();
		private object blobSync = new object();

		public static int width, height;
		private int window_width =  640;
		private int window_height = 480;
		private int window_left = 0;
		private int window_top = 0;
		private int screen_width = Screen.PrimaryScreen.Bounds.Width;
		private int screen_height = Screen.PrimaryScreen.Bounds.Height;

		private bool fullscreen;
		private bool verbose;

		SolidBrush blackBrush = new SolidBrush(Color.Black);
		SolidBrush whiteBrush = new SolidBrush(Color.White);

		SolidBrush grayBrush = new SolidBrush(Color.Gray);
		Pen fingerPen = new Pen(new SolidBrush(Color.Blue), 1);

		private List<Figure> figures = new List<Figure>();



		public void MakeFigures()
    {
		Figure fig = new Figure("apple.jpg",0);
		fig.secondMarkerId = 3;
	
		figures.Add(fig);

		fig  = new Figure("cat.jpg", 1);
		fig.secondMarkerId = 4;
		figures.Add(fig);
		fig = new Figure(4, "cat");
		fig.brush = new SolidBrush(Color.Blue);
		figures.Add(fig);
		fig = new Figure(3, "apple");



		fig.brush = new SolidBrush(Color.Blue);
		figures.Add(fig);

		fig = new Figure(10, "interface");



		fig.brush = new SolidBrush(Color.Blue);


		figures.Add(fig);

	}


		public TuioDemo(int port) {
		
			verbose = true;
			fullscreen = false;
			width = window_width;
			height = window_height;
		MakeFigures();
			this.ClientSize = new System.Drawing.Size(width, height);
			this.Name = "TuioDemo";
			this.Text = "TuioDemo";
		transmitter = new OSCTransmitter("127.0.0.1", port);
		OSCPacket packet;
 
			this.Closing+=new CancelEventHandler(Form_Closing);
			this.KeyDown +=new KeyEventHandler(Form_KeyDown);

			this.SetStyle( ControlStyles.AllPaintingInWmPaint |
							ControlStyles.UserPaint |
							ControlStyles.DoubleBuffer, true);

			objectList = new Dictionary<long,TuioDemoObject>(128);
			cursorList = new Dictionary<long,TuioCursor>(128);
			
			client = new TuioClient(port);
			client.addTuioListener(this);

			client.connect();
		}

		private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {

 			if ( e.KeyData == Keys.F1) {
	 			if (fullscreen == false) {

					width = screen_width;
					height = screen_height;

					window_left = this.Left;
					window_top = this.Top;

					this.FormBorderStyle = FormBorderStyle.None;
		 			this.Left = 0;
		 			this.Top = 0;
		 			this.Width = screen_width;
		 			this.Height = screen_height;

		 			fullscreen = true;
	 			} else {

					width = window_width;
					height = window_height;

		 			this.FormBorderStyle = FormBorderStyle.Sizable;
		 			this.Left = window_left;
		 			this.Top = window_top;
		 			this.Width = window_width;
		 			this.Height = window_height;

		 			fullscreen = false;
	 			}
 			} else if ( e.KeyData == Keys.Escape) {
				this.Close();

 			} else if ( e.KeyData == Keys.V ) {
					
 				verbose=!verbose;
			Console.WriteLine("add obbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
 			}

 		}
	public void setMarkerInfo(string label, Bitmap img, int id)
    {
		Figure fig = new Figure(img, id);
		
		fig.secondMarkerId = id+1;
		figures.Add(fig);
		Figure fig2 = new Figure(id + 1, label);
		fig2.brush = new SolidBrush(Color.Blue); 
		figures.Add(fig2);
	}
		private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			client.removeTuioListener(this);

			client.disconnect();
			System.Environment.Exit(0);
		}

		public void addTuioObject(TuioObject o) {
			lock(objectSync) {
				objectList.Add(o.SessionID,new TuioDemoObject(o));
			} if (verbose) Console.WriteLine("add obj "+o.SymbolID+" ("+o.SessionID+") "+o.X+" "+o.Y+" "+o.Angle);
		}

		public void updateTuioObject(TuioObject o) {
			lock(objectSync) {
				objectList[o.SessionID].update(o);
			}
			if (verbose) Console.WriteLine("set obj "+o.SymbolID+" "+o.SessionID+" "+o.X+" "+o.Y+" "+o.Angle+" "+o.MotionSpeed+" "+o.RotationSpeed+" "+o.MotionAccel+" "+o.RotationAccel);
		}

		public void removeTuioObject(TuioObject o) {
			lock(objectSync) {
				objectList.Remove(o.SessionID);
			}
			if (verbose) Console.WriteLine("del obj "+o.SymbolID+" ("+o.SessionID+")");
		}

		public void addTuioCursor(TuioCursor c) {
			lock(cursorSync) {
				cursorList.Add(c.SessionID,c);
			}
			if (verbose) Console.WriteLine("add cur "+c.CursorID + " ("+c.SessionID+") "+c.X+" "+c.Y);
		}

		public void updateTuioCursor(TuioCursor c) {
			if (verbose) Console.WriteLine("set cur "+c.CursorID + " ("+c.SessionID+") "+c.X+" "+c.Y+" "+c.MotionSpeed+" "+c.MotionAccel);
		}

		public void removeTuioCursor(TuioCursor c) {
			lock(cursorSync) {
				cursorList.Remove(c.SessionID);
			}
			if (verbose) Console.WriteLine("del cur "+c.CursorID + " ("+c.SessionID+")");
 		}

		public void addTuioBlob(TuioBlob b) {
			lock(blobSync) {
				blobList.Add(b.SessionID,b);
			}
		if (verbose) Console.WriteLine("add blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area);
		}

		public void updateTuioBlob(TuioBlob b) {
		if (verbose) Console.WriteLine("set blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area+" "+b.MotionSpeed+" "+b.RotationSpeed+" "+b.MotionAccel+" "+b.RotationAccel);
		}

		public void removeTuioBlob(TuioBlob b) {
			lock(blobSync) {
				blobList.Remove(b.SessionID);
			}
			if (verbose) Console.WriteLine("del blb "+b.BlobID + " ("+b.SessionID+")");
		}

		public void refresh(TuioTime frameTime) {
			Invalidate();
		}
		
		public void playSound( string text)
    {
		SpeechSynthesizer reader = new SpeechSynthesizer();
		reader.Rate = (int)-2;
		reader.Speak(text);

	}



        		 
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// Getting the graphics object
			Graphics g = pevent.Graphics;
		Font font = new Font("Arial", 20.0f);
		g.FillRectangle(whiteBrush, new Rectangle(0,0,width,height));

			// draw the cursor path
			if (cursorList.Count > 0) {
 			 lock(cursorSync) {
			 foreach (TuioCursor tcur in cursorList.Values) {
					List<TuioPoint> path = tcur.Path;
					TuioPoint current_point = path[0];

					for (int i = 0; i < path.Count; i++) {
						TuioPoint next_point = path[i];
						g.DrawLine(fingerPen, current_point.getScreenX(width), current_point.getScreenY(height), next_point.getScreenX(width), next_point.getScreenY(height));
						current_point = next_point;
					}
					g.FillEllipse(grayBrush, current_point.getScreenX(width) - height / 100, current_point.getScreenY(height) - height / 100, height / 50, height / 50);
		 
					g.DrawString(tcur.CursorID + "", font, blackBrush, new PointF(tcur.getScreenX(width) - 10, tcur.getScreenY(height) - 10));
				}
			}
		 }

			// draw the objects
			if (objectList.Count > 0)
			{
			lock (objectSync)
			{
				//foreach (TuioDemoObject tobject in objectList.Values)
				//{
				//	tobject.paint(g);
					 
				//}




				foreach (TuioDemoObject tobject in objectList.Values)
				{
					 
					int x = (int)(tobject.X * TuioDemo.width);
					int y = (int)(tobject.Y * TuioDemo.height);

			
					for (int j = 0; j < figures.Count; j++)
					{
						 
						if (tobject.SymbolID == figures[j].MarkerId)
						{
							
							if (figures[j].MarkerId == 10)
                            {
								if (obj == null)
                                {
									obj = new Form1(this);
									obj.Show();

								}
							
                            }
                            else
                            {
								if (figures[j].type == 0)
								{

									figures[j].x = x;
									figures[j].y = y;
									g.DrawImage(figures[j].img, x, y);

								}
								if (figures[j].type == 1)
								{
									figures[j].x = x;
									figures[j].y = y;
									g.FillRectangle(figures[j].brush, x, y, 100, 100);
									g.DrawString(figures[j].word, font, Brushes.Black, x + 25, y + 25);
									figures[j].brush = new SolidBrush(Color.Blue);

								}
							}

							



						}
					}
				}


				 
				for (int i = 0; i < figures.Count; i++)
                {
					for (int j=0; j<figures.Count;j++)
                    {
						if ( j == i)
                        {
							continue;
                        }
						if ((figures[i].x > figures[j].x && figures[i].x < figures[j].x+100) ||
							(figures[i].x+100 > figures[j].x && figures[i].x+100 < figures[j].x + 100))
                        {
							if ((figures[i].y > figures[j].y && figures[i].y < figures[j].y + 100)||
								(figures[i].y+100 > figures[j].y && figures[i].y+100 < figures[j].y + 100)
								)
                            {
								if (figures[i].type != figures[j].type)
                                {
									if (figures[i].type == 1 )
                                    {
										figures[i].brush = new SolidBrush(Color.Red);
                                    }
                                    else
                                    {
										figures[j].brush = new SolidBrush(Color.Red);
									}
									if (figures[i].secondMarkerId == figures[j].MarkerId)
									{
										figures[j].brush = new SolidBrush(Color.Green);

										if (!figures[j].played)
                                        {
											figures[j].played = true;
											playSound(figures[j].word);

										}
										

									}

									 

									if (figures[i].MarkerId == figures[j].secondMarkerId)
                                    {
										figures[i].brush = new SolidBrush(Color.Green);
										if (!figures[i].played)
										{
											figures[i].played = true;
											playSound(figures[i].word);

										}

										 
									}
                                }
                            }

						}
                    }
                }
				for (int i = 0; i < figures.Count; i++)
				{
					figures[i].x = -1000;
					
				}

			}
			}
		}

	 
	}
