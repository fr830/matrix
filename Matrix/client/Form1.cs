using System;
using System.Windows.Forms;
using System.Runtime.Remoting;
using Matrix.Types;



namespace client
{
    public partial class Form1 : Form
    {
        IServer srv = null;
        DateTime _lastupdate;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WellKnownClientTypeEntry[] tps = RemotingConfiguration.GetRegisteredWellKnownClientTypes();
            srv = (IServer)Activator.GetObject(typeof(IServer), tps[0].ObjectUrl);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tag tag = new Tag();
            tag.Name = "arhive";
            tag.Value = Convert.ToBoolean(textBox1.Text);
            try
            {
                srv.WriteTag(tag);
                Tag newtag = srv.ReadTag("arhive");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox2.Text = Convert.ToString(srv.ReadTag("k5.xim.c").Value);
            textBox3.Text = Convert.ToString(srv.ReadTag("k5.xim.p").Value);
            textBox4.Text = Convert.ToString(srv.ReadTag("k5.xim.al").Value);
            //textBox5.Text = Convert.ToString(srv.ReadTag("pozkovsh").Value);
            //textBox6.Text = Convert.ToString(srv.ReadTag("tag17").Value);
            //Tag tag7 = srv.ReadTag("[tag7]");
            //textBox8.Text = Convert.ToString(tag7.Value)+"["+tag7.DtChange.ToShortTimeString()+"]";
            
            //string[] tagName = { "[tag1]", "[tag2]", "[tag3]", "[tag4]", "[tag5]", "[tag6]", "[tag7]", "[tag8]"};
            //Tag[] tags = srv.ReadTags(ref _lastupdate,tagName);
            //xml.Text = "<root>\n";
            //if (tags != null)
            //{
            //    foreach (Tag tag in tags)
            //    {
            //        xml.AppendText("    <" + tag.Name + ">" + tag.Value + "</" + tag.Name + ">\n");
            //    }
            //}
            //xml.AppendText("</root>\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            localhost.tags ws = new localhost.tags();
            string xml = ws.ReadTags("k5.group");
            textBox8.Text = xml;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            localhost.tags ws = new localhost.tags();
            ws.WriteTags(xml.Text);
        }
    }
}