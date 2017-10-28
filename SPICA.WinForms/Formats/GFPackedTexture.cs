using SPICA.Formats.Common;
using System.IO;
using System.Text;
using SPICA.Formats.GFL2.Texture;
using SPICA.Formats.CtrH3D;
using System.Collections.Generic;
using System.Linq;

namespace SPICA.WinForms.Formats
{
    class GFPackedTexture
    {
		private List<GFTexture> Textures;

		public GFPackedTexture() {
			Textures = new List<GFTexture>();
		}

		public GFPackedTexture(H3D Scene, int index = -1) : this() {
			if (index >= 0) {
				Textures.Add(new GFTexture(Scene.Textures[index]));
			} else {
				foreach (var t in Scene.Textures) Textures.Add(new GFTexture(t));
			}
		}

        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header, int StartIndex)
        {
            H3D Output = new H3D();

            //Textures and animations
            for (int i = StartIndex; i < Header.Entries.Length; i++)
            {
                byte[] Buffer = new byte[Header.Entries[i].Length];

                Input.Seek(Header.Entries[i].Address, SeekOrigin.Begin);

                Input.Read(Buffer, 0, Buffer.Length);

                if (Buffer.Length < 4 || Encoding.ASCII.GetString(Buffer, 0, 4) != "BCH\0") continue;

                using (MemoryStream MS = new MemoryStream(Buffer))
                {
                    Output.Merge(H3D.Open(MS));
                }
            }

            return Output;
        }

		public void Save(string FileName) {
			using (var frm = new FrmGFTFormat()) {
				if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					using (BinaryWriter br = new BinaryWriter(new FileStream(FileName + (frm.Format == 0 ? ".pc" : ".bin"), FileMode.Create))) {
						//Write container
						int currPos = 0x80;
						switch (frm.Format) {
							case -1: return;
							case 0: 
							{
								//header
								br.WritePaddedString("PC", 2);
								br.Write((short)Textures.Count);
								
								//position entries
								for (int i = 0; i < Textures.Count; i++) {
									br.Write(currPos);
									currPos += Textures[i].RawBuffer.Count() + 0x80;
								}
								br.Write(currPos);
								
								//padding
								for (int i = Textures.Count; i < 30; i++) {
									br.Write(0);
								}
								break;
							}
						}
						//Write body
						foreach (var t in Textures) t.Write(br);
					}
				}
			}
		}
	}
}
