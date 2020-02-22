﻿namespace StbTrueTypeSharp
{
	partial class StbTrueType
	{
		public static byte stbtt__buf_get8(stbtt__buf b)
		{
			if ((b.cursor) >= (b.size))
				return (byte)(0);
			return (byte)(b.data[b.cursor++]);
		}

		public static byte stbtt__buf_peek8(stbtt__buf b)
		{
			if ((b.cursor) >= (b.size))
				return (byte)(0);
			return (byte)(b.data[b.cursor]);
		}

		public static void stbtt__buf_seek(stbtt__buf b, int o)
		{
			b.cursor = (int)((((o) > (b.size)) || ((o) < (0))) ? b.size : o);
		}

		public static void stbtt__buf_skip(stbtt__buf b, int o)
		{
			stbtt__buf_seek(b, (int)(b.cursor + o));
		}

		public static uint stbtt__buf_get(stbtt__buf b, int n)
		{
			uint v = (uint)(0);
			int i = 0;
			for (i = (int)(0); (i) < (n); i++)
			{
				v = (uint)((v << 8) | stbtt__buf_get8(b));
			}
			return (uint)(v);
		}

		public static stbtt__buf stbtt__new_buf(FakePtr<byte> p, ulong size)
		{
			stbtt__buf r = new stbtt__buf();
			r.data = p;
			r.size = ((int)(size));
			r.cursor = (int)(0);
			return (stbtt__buf)(r);
		}

		public static stbtt__buf stbtt__buf_range(stbtt__buf b, int o, int s)
		{
			stbtt__buf r = (stbtt__buf)(stbtt__new_buf(FakePtr<byte>.Null, (ulong)(0)));
			if (((((o) < (0)) || ((s) < (0))) || ((o) > (b.size))) || ((s) > (b.size - o)))
				return (stbtt__buf)(r);
			r.data = b.data + o;
			r.size = (int)(s);
			return (stbtt__buf)(r);
		}

		public static stbtt__buf stbtt__cff_get_index(stbtt__buf b)
		{
			int count = 0;
			int start = 0;
			int offsize = 0;
			start = (int)(b.cursor);
			count = (int)(stbtt__buf_get((b), (int)(2)));
			if ((count) != 0)
			{
				offsize = (int)(stbtt__buf_get8(b));
				stbtt__buf_skip(b, (int)(offsize * count));
				stbtt__buf_skip(b, (int)(stbtt__buf_get(b, (int)(offsize)) - 1));
			}

			return (stbtt__buf)(stbtt__buf_range(b, (int)(start), (int)(b.cursor - start)));
		}

		public static uint stbtt__cff_int(stbtt__buf b)
		{
			int b0 = (int)(stbtt__buf_get8(b));
			if (((b0) >= (32)) && (b0 <= 246))
				return (uint)(b0 - 139);
			else if (((b0) >= (247)) && (b0 <= 250))
				return (uint)((b0 - 247) * 256 + stbtt__buf_get8(b) + 108);
			else if (((b0) >= (251)) && (b0 <= 254))
				return (uint)(-(b0 - 251) * 256 - stbtt__buf_get8(b) - 108);
			else if ((b0) == (28))
				return (uint)(stbtt__buf_get((b), (int)(2)));
			else if ((b0) == (29))
				return (uint)(stbtt__buf_get((b), (int)(4)));
			return (uint)(0);
		}

		public static void stbtt__cff_skip_operand(stbtt__buf b)
		{
			int v = 0;
			int b0 = (int)(stbtt__buf_peek8(b));
			if ((b0) == (30))
			{
				stbtt__buf_skip(b, (int)(1));
				while ((b.cursor) < (b.size))
				{
					v = (int)(stbtt__buf_get8(b));
					if (((v & 0xF) == (0xF)) || ((v >> 4) == (0xF)))
						break;
				}
			}
			else
			{
				stbtt__cff_int(b);
			}
		}

		public static stbtt__buf stbtt__dict_get(stbtt__buf b, int key)
		{
			stbtt__buf_seek(b, (int)(0));
			while ((b.cursor) < (b.size))
			{
				int start = (int)(b.cursor);
				int end = 0;
				int op = 0;
				while ((stbtt__buf_peek8(b)) >= (28))
				{
					stbtt__cff_skip_operand(b);
				}
				end = (int)(b.cursor);
				op = (int)(stbtt__buf_get8(b));
				if ((op) == (12))
					op = (int)(stbtt__buf_get8(b) | 0x100);
				if ((op) == (key))
					return (stbtt__buf)(stbtt__buf_range(b, (int)(start), (int)(end - start)));
			}
			return (stbtt__buf)(stbtt__buf_range(b, (int)(0), (int)(0)));
		}

		public static void stbtt__dict_get_ints(stbtt__buf b, int key, int outcount, FakePtr<uint> _out_)
		{
			int i = 0;
			stbtt__buf operands = (stbtt__buf)(stbtt__dict_get(b, (int)(key)));
			for (i = (int)(0); ((i) < (outcount)) && ((operands.cursor) < (operands.size)); i++)
			{
				_out_[i] = (uint)(stbtt__cff_int(operands));
			}
		}

		public static int stbtt__cff_index_count(stbtt__buf b)
		{
			stbtt__buf_seek(b, (int)(0));
			return (int)(stbtt__buf_get((b), (int)(2)));
		}

		public static stbtt__buf stbtt__cff_index_get(stbtt__buf b, int i)
		{
			int count = 0;
			int offsize = 0;
			int start = 0;
			int end = 0;
			stbtt__buf_seek(b, (int)(0));
			count = (int)(stbtt__buf_get((b), (int)(2)));
			offsize = (int)(stbtt__buf_get8(b));
			stbtt__buf_skip(b, (int)(i * offsize));
			start = (int)(stbtt__buf_get(b, (int)(offsize)));
			end = (int)(stbtt__buf_get(b, (int)(offsize)));
			return (stbtt__buf)(stbtt__buf_range(b, (int)(2 + (count + 1) * offsize + start), (int)(end - start)));
		}

		public static stbtt__buf stbtt__get_subrs(stbtt__buf cff, stbtt__buf fontdict)
		{
			uint subrsoff = (uint)(0);

			uint[] private_loc = new uint[2];
			private_loc[0] = (uint)(0);
			private_loc[1] = (uint)(0);

			stbtt__buf pdict = new stbtt__buf();
			stbtt__dict_get_ints(fontdict, (int)(18), (int)(2), new FakePtr<uint>(private_loc));
			if ((private_loc[1] == 0) || (private_loc[0] == 0))
				return (stbtt__buf)(stbtt__new_buf(FakePtr<byte>.Null, (ulong)(0)));
			pdict = (stbtt__buf)(stbtt__buf_range(cff, (int)(private_loc[1]), (int)(private_loc[0])));
			stbtt__dict_get_ints(pdict, (int)(19), (int)(1), new FakePtr<uint>(subrsoff));
			if (subrsoff == 0)
				return (stbtt__buf)(stbtt__new_buf(FakePtr<byte>.Null, (ulong)(0)));
			stbtt__buf_seek(cff, (int)(private_loc[1] + subrsoff));
			return (stbtt__buf)(stbtt__cff_get_index(cff));
		}

		public static stbtt__buf stbtt__get_subr(stbtt__buf idx, int n)
		{
			int count = (int)(stbtt__cff_index_count(idx));
			int bias = (int)(107);
			if ((count) >= (33900))
				bias = (int)(32768);
			else if ((count) >= (1240))
				bias = (int)(1131);
			n += (int)(bias);
			if (((n) < (0)) || ((n) >= (count)))
				return (stbtt__buf)(stbtt__new_buf(FakePtr<byte>.Null, (ulong)(0)));
			return (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(idx), (int)(n)));
		}
	}
}