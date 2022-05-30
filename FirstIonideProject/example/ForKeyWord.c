void main(int n)
{
  int i = 0;
  int j = 0;

  for (i = 0; i < n; i = i + 1)
  {
    printf("%d：", i);
    for (j = 0; j < i; j = j + 1)
    {
      printf("%d ", j);
    }
    printf("\n");
  }
}