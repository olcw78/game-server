using System.Data;

namespace gs.shared;

public static class StringExtensions {
  public static string? NullIfEmpty(this string s) =>
    string.IsNullOrEmpty(s) ? null : s;

  public static unsafe string? Reversed(this string s) {
    int len = s.Length;

    char tmp;
    int mid = len / 2;
    int i = 0;

    fixed (char* p = s) {
      while (i < mid) {
        int offset = len - i;

        tmp = *p;
        *p = *(p + offset);
        *(p + offset) = tmp;

        ++i;
      }

      string ret = p->ToString();
      return ret.NullIfEmpty();
    }
  }
}