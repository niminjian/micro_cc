
int x;
int g()
{
  print x;
}

int f()
{
  int x = 3;
  return g();
}

int main()
{
  x = 10;
  f();
}