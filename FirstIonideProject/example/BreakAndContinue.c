void main(int n)
{
  int i;
  i = 0;
  while (i < 5)
  {
    if (i == 1)
    {
      i = i + 1;
      continue;
    }
    print i; // 测试1
    // break; // 测试2

    i = i + 1;
  }
}
