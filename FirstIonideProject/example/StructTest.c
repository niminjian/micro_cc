struct test
{
  int x;
  char c;
  int a[3];
};

void main()
{
  struct test t;
  t.x = 200;
  t.c = 'a';
//  printf("%d\n", t.x);
//  printf("%c\n", t.c);
  print t.x;
  print t.c; 
}
