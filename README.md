

[2021-2022学年第2学期]

# [**实 验 报 告**]

![zucc](./README.assets/zucc.png)

- 课程名称:编程语言原理与编译
- 实验项目:期末大作业
- 专业班级__计算机1901_
- 学生学号_ 31904027_ 31903139_
- 学生姓名 倪敏建  许周毅
- 实验指导教师:张芸



# 1.简介

​	编译原理是计算机专业的一门重要专业课，旨在介绍编译程序构造的一般原理和基本方法。内容包括语言和文法、词法分析、语法分析、语法制导翻译、中间代码生成、存储管理、代码优化和目标代码生成。 编译原理是计算机专业设置的一门重要的专业课程。编译原理课程是计算机相关专业学生的必修课程和高等学校培养计算机专业人才的基础及核心课程，同时也是计算机专业课程中最难及最挑战学习能力的课程之一。	

​	一开始接触这个大作业，有些手足无措，涉及的内容太过繁杂，与之前的知识体系没有接轨，学习成本过高。后仔细研读老师提供的材料，逐渐被这门课所吸引，原来这个世界上存在着这么多有趣的语言，它们有各自的特色，有些为特殊的使用场景设计。

​	microC的设计更是令人惊叹，我们最终选择了它作为我们的大作业方向。

#  2.语言手册

## 解释器

```shell
dotnet restore  interpc.fsproj
dotnet clean  interpc.fsproj
dotnet build -v n interpc.fsproj
dotnet run -p interpc.fsproj .\example\commentaryTest.c 
```

## 编译器

```shell
#虚拟机编译
gcc machine.c -o machine 
```

```shell
dotnet restore  microc.fsproj #恢复项目的依赖项和工具
dotnet clean  microc.fsproj #清除项目输出
dotnet build  microc.fsproj #生成项目及其所有依赖项

dotnet run --project microc.fsproj .\example\xxx.c xxx(可选)  # 编译 xxx表示输入的数据，下同
.\machine.exe .\example\xxx.out xxx  # 执行（通过虚拟机执行）

###相关说明：
已弃用使用缩写“-p”来代表“--project”。请使用“--project”。
framework 'Microsoft.NETCore.App', version '5.0.0' (x64)
###
```

## 优化

```shell
#microc-->microcc
dotnet restore  microcc.fsproj
dotnet clean  microcc.fsproj #！如果有异常结果出现，先考虑清除项目输出
dotnet build  microcc.fsproj

dotnet run --project microcc.fsproj .\example\xxx.c xxx(可选)  # 编译 xxx表示输入的数据
.\machine.exe .\example\xxx.out xxx(可选)  # 执行（通过虚拟机执行）
```

## 中间过程

```shell
###----------编译器----------
-- dotnet fsi
-- #r "nuget: FsLexYacc";;
-- #load "Absyn.fs"  "CPar.fs" "CLex.fs" "Debug.fs" "Parse.fs" "Machine.fs" "Backend.fs" "Comp.fs" "ParseAndComp.fs";; 
-- open ParseAndComp;;
-- fromFile "example\xxx.c"
-- compileToFile (fromFile "example\xxx.c") "xxx";; #有点问题
-- #q;;

###----------优化编译器----------
-- dotnet fsi
-- #r "nuget: FsLexYacc";;
-- #load "Absyn.fs"  "CPar.fs" "CLex.fs" "Debug.fs" "Parse.fs" "Machine.fs" "Backend.fs" "Contcomp.fs" "ParseAndContcomp.fs";;
-- open ParseAndContcomp;;
#-----查看中间AST生成-----（抽象语法树）
-- fromFile "example\xxx.c";; 
#优化编译虚拟指令序列
-- contCompileToFile (fromFile "example\xxx.c") "xxx.out";;
-- #q;;
```

# 3.结构设计

前端：由`F#`语言编写而成  

- `Absyn.fs`: 抽象语法树结构的定义，定义变量描述、函数和类型的构造方法
- `CLex.fsl`: 词法定义(将输入分解成一个个独立的词法符号)
  + CLex 中定义基本的关键字、标识符、常量、进制转化函数、转移函数等，遇到对应字符会模式匹配到目标字符，然后就给 CPar 处理
- `CPar.fsy`: 语法定义(分析程序的短语结构)
  + CPar 文件分为两部分
  + 第一部分声明需要使用的变量(词元)，声明变量后还需要声明优先级
  + 第二部分定义语法规则(文法)包括 : statement ,expression ,function ,main ,vardeclare variabledescirbe ,type ,const这些基本元素
  + 表示识别到前面定义的这些大写字母组成的符号串后,怎么处理这些规则
- `CPas.fsy`: 语义分析(推算程序的含义)
- `Parse.fs`: 语法解析器（从文件或字符串中获取抽象语法树）
- `Interp.fs`: 解释器
- `Comp.fs`：编译器(将高级语言翻译为低级语言)
  - 相关：System.IO、Absyn.fs、Machine.fs、Debug.fs、Backend.fs、microc.fs、microc.fsproj
- `Machine.fs`：栈式虚拟机，machine.c
- `Contcomp.fs`: 优化编译器
- `example`:存放测试程序



# 4.测试方案

## 4.1 词法功能

### 4.1.1 注释表示方式

- 实现注释表达方式  `//` 	`/* */`    `(* *)`

- 测试样例 (commentaryTest.c)

  ```c
  void main()
  {
    int i = 5;
    (*for (; i < 10; i++); *)
        printf i;
  }
  ```
  
- 测试结果

解释：

![](./README.assets/interpreter/1.png)

编译：

![](./README.assets/compile/1.png)



中间过程：（中间过程用优化编译器测试，下同）

查看中间抽象语法树(AST)生成：

![ast](./README.assets/ast/1.png)

优化编译虚拟指令序列：

虚拟指令序列:

![seq](./README.assets/seq/1.png)	



### 4.1.2 标识符定义

- 标识符定义方式：允许_开头

- 测试样例 (IdentifierDefinition.c)

```
void main()
{
  int _x = 1;
  int _y = 2;
//  printf("%d\n", _x);
//  printf("%d", _y);
	print _x;
	print _y;
}
```

- 测试结果


解释

![2](./README.assets/interpreter/2.png)

编译：

![](./README.assets/compile/2.png)

中间过程：

![ast](./README.assets/ast/2.png)

虚拟指令序列：

![seq](./README.assets/seq/2.png)

### 4.1.3 进制转换

- 进制转换：0b-二进制、0o-八进制、十进制、0x-十六进制

- 测试样例 (RadixConversion.c)

  ```
  void main()
  {
    int x = 0b111;
    int y = 0o111;
    int z = 111;
    int k = 0x111;
    print x;
    print y;
    print z;
    print k;
  }
  ```

- 测试结果 


解释

![3](./README.assets/interpreter/3.png)

编译：

![](./README.assets/compile/3.png)

中间过程：

![ast](./README.assets/ast/3.png)

![seq](./README.assets/seq/3.png)

## 4.2 语法功能

### 4.2.1 for循环

- 测试for的循环功能

- 测试样例 (ForKeyWord.c)

  ```
  void main(int n)
  {
    int i = 0;
    int j = 0;
  
    for (i = 0; i < n; i = i + 1)
    {
      print i;
      for (j = 0; j < i; j = j + 1)
      {
        print j;
      }
    }
  }
  ```
  
- 测试结果


解释

![4](./README.assets/interpreter/4.png)

编译：

![](./README.assets/compile/4.png)

中间过程：

![ast](./README.assets/ast/4.png)

![seq](./README.assets/seq/4.png)

### 4.2.2 do-while循环实现

- 测试do-while的循环功能

- 测试样例 (DoWhileKeyWord.c)

  ```
  void main(int n)
  {
    int x = 0;
    do
    {
      x += 2;
      print x;
      print " "
    } while (x < n);
  }
  ```

- 测试结果


解释

![5](./README.assets/interpreter/5.png)

编译：

![](./README.assets/compile/5.png)

中间过程：

![ast](./README.assets/ast/5.png)

![seq](./README.assets/seq/5.png)

### 4.2.3 switch-case判断

- 测试switch-case的判断功能

- 测试样例 (SwitchCaseKeyWord.c)

  ```
  void main(int n)
  {
    switch (n)
    {
    case 0:
      print 0;
    case 1:
      print 1;
    default:
      print - 1;
    }
  }
  
  ```

- 测试结果


解释

![6](./README.assets/interpreter/6.png)

编译：

![](./README.assets/compile/6.png)

中间过程：

![ast](./README.assets/ast/6.png)

![seq](./README.assets/seq/6.png)



### 4.2.4 break与continue语句

- 测试break和continue的功能

- 测试样例 (BreakAndContinue.c)

  ```
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
  ```

  编译：
  
  ![](./README.assets/compile/7.png)
  
  中间过程：
  
  ![ast](./README.assets/ast/7.png)
  
  虚拟指令序列：
  
  ![seq](./README.assets/seq/7.png)

### 4.2.5 三目运算符

- 测试三目运算符的功能

- 测试样例 (TernaryOperator.c)

  ```
  void main(int n)
  {
    n == 10 ? print 2 : print 5;
  }
  ```

- 测试结果

  解释

  ![8](./README.assets/interpreter/8.png)

编译：

![](./README.assets/compile/8.png)

中间过程：

![ast](./README.assets/ast/8.png)

虚拟指令序列：

![seq](./README.assets/seq/8.png)

### 4.2.6 printf

- 测试样例 (printf.c)

  ```
  void main(int n)
  {
    printf("hello %d", n);
  }
  ```

- 测试结果

  解释

  ![9](./README.assets/interpreter/9.png)

  编译：
  
  ![](./README.assets/compile/9.png)
  
  中间过程：
  
  ![ast](./README.assets/ast/9.png)
  
  虚拟指令序列：
  
  ![seq](./README.assets/seq/9.png)

### 4.2.7 do-until

- 测试样例 (dountil.c)

  ```
  void main()
  {
    int i = 0;
    do
    {
      print i;
      i++;
    }
    until(i == 10);
  }
  ```

  测试结果

  解释

  ![10](./README.assets/interpreter/10.png)

编译：

![](./README.assets/compile/10.png)

中间过程：

![ast](./README.assets/ast/10.png)

虚拟指令序列：

![seq](./README.assets/seq/10.png)

### 4.2.8 String

- 测试样例 (StringTest.c)

  ```
  void main()
  {
    String s;
    s = "Manchester City";
    printf("hello %s\n", s);
  }
  ```

- 测试结果

  解释

  ![11](./README.assets/interpreter/11.png)

编译：

![](./README.assets/compile/11.png)

中间过程：

![ast](./README.assets/ast/11.png)

虚拟指令序列：

![seq](./README.assets/seq/11.png)

## 4.3 语义功能

### 4.3.1 静态作用域

- 作用域：变量是名字与实体的绑定，一段程序代码中所用到的名字并不总是有效的，而限定这个变量名的可用代码范围就是这个名字的作用域。其中分为动态作用域和静态作用域，动态作用域指函数的作用域是在函数被调用时才决定的，而静态作用域则是在编译时就已经决定了。

- 本语言采用静态作用域规则。如下样例，先将全局变量x进行赋值，随后调用 f() ，在函数f中定义局部变量x，并在函数f内部调用 g() ，查看在函数g中打印出的变量x值为函数f中的局部变量值还是全局变量值来判断是静态还是动态。

- 测试样例 (StaticTest.c)

  ```
  
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
  ```

- 测试结果

  解释（在测试样例中，声明了一个全局变量x，后再函数中重新声明变量，尝试进行修改）

  ![12](./README.assets/interpreter/12.png)

编译：

![](./README.assets/compile/12.png)

中间过程：

![ast](./README.assets/ast/12.png)

虚拟指令序列：

![seq](./README.assets/seq/12.png)

## 4.4 特性功能

### 4.4.1 位运算

- 实现位运算&与， | 或， ^ 异或，~ 取反，<< ，左移，>> 右移

- 测试样例 (Bitwise.c)

  ```
  void main()
  {
    int a;
    int b;
    int c;
    a = 1;
    b = 0;
    c = a & b;
    print c;
    c = a | b;
    print c;
    c = a << 2;
    print c;
    c = c >> 1;
    print c;
    c = a ^ b;
    print c;
    c = ~b;
    print c;
  }
  ```

- 测试结果

  解释

  ![15](./README.assets/interpreter/15.png)

编译：

![](./README.assets/compile/13.png)

中间过程：

![ast](./README.assets/ast/13.png)

虚拟指令序列：

![seq](./README.assets/seq/13.png)

### 4.4.2 逻辑运算

- 实现逻辑运算== 等于，!= 不等于，< 小于，<= 小于等于，> 大于，>= 大于等于，&& 与，|| 或，! 非

- 测试样例 (LogicalOperation.c)

  ```
  void main(){
      print (0==4)&&(2!=1);  // 0
      print (3>6)||(6<10);    // 1
      print (2>=1)&&(3<=7);   // 1
      print !(9>4);           // 0
  }
  ```

- 测试结果

  解释

  ![16](./README.assets/interpreter/16.png)

编译：

![](./README.assets/compile/14.png)

中间过程：

![ast](./README.assets/ast/14.png)

虚拟指令序列：

![seq](./README.assets/seq/14.png)

### 4.4.3 运算符

- 实现 i++，i--，++i，--i 自增自减运算符

- 测试样例 (Operator.c)

  ```
  void main(int n) { 
    print n;
    print ++n;
    print --n;
    print n++;
    print n--;
  }
  ```

- 测试结果

  解释

  ![17](./README.assets/interpreter/17.png)

编译：

![](./README.assets/compile/15.png)

中间过程：

![ast](./README.assets/ast/15.png)

虚拟指令序列：

![seq](./README.assets/seq/15.png)

### 4.4.4 复合赋值运算符

- 测试样例 (CompoundAssignment.c)

  ```
  void main(int n)
  {
    n += 2;
    print n;
    n -= 2;
    print n;
    n *= 2;
    print n;
    n %= 2;
    print n;
  }
  ```

- 测试结果

  解释

  ![18](./README.assets/interpreter/18.png)

编译：

![](./README.assets/compile/16.png)

中间过程：

![ast](./README.assets/ast/16.png)

虚拟指令序列：

![seq](./README.assets/seq/16.png)

### 4.4.5 struct结构

首先，先创建结构体定义表，用来查找，结构体定义表中包含结构体的总体大小，名字，以及变量和偏移量。然后查找结构体变量表，加入该变量到varEnv中，访问成员时，便可以通过.运算符，通过偏移值转化为简单指令集。

- 测试样例 (StructTest.c)

  ```
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
    printf t.x;
    printf t.c;
  }
  ```

- 测试结果

  解释

  ![19](./README.assets/interpreter/19.png)

编译：

![](./README.assets/compile/17.png)

中间过程：

![ast](./README.assets/ast/17.png)

虚拟指令序列：

![seq](./README.assets/seq/17.png)

### 4.4.6 数组

- 实现数组 `int[]` ，`char[]`

- 测试样例 (Array.c)

  ```
  int main(){
      int i;
      int a[10];
      char c[5];
      c[0] = 'h';
      c[1] = 'e';
      c[2] = 'l';
      c[3] = 'l';
      c[4] = 'o';
      for(i = 0; i < 10; ++i){
          a[i] = i;
      }
      for(i = 0; i < 10; ++i){
          print a[i];
      }
      println;
      for(i = 0; i < 5; ++i){
          print c[i];
      }
  }
  ```

- 测试结果

  解释

  ![20](./README.assets/interpreter/20.png)

编译：

![](./README.assets/compile/18.png)

中间过程：

![ast](./README.assets/ast/18.png)

虚拟指令序列：

![seq](./README.assets/seq/18.png)



### 4.4.7 自定义函数

- 模仿自定义函数的过程实现

- 测试样例 (SelfDefinitionFunc.c)

  ```
  void myFunc(int i)
  {
    print i * 10;
  }
  
  int main(int n)
  {
    myFunc(n);
  }
  ```

- 测试结果

  解释

  ![21](./README.assets/interpreter/21.png)

编译：

![](./README.assets/compile/19.png)

中间过程：

![ast](./README.assets/ast/19.png)

虚拟指令序列：

![seq](./README.assets/seq/19.png)

### 4.4.8 基本类型实现

- 实现变量类型 `int` 整数，`float` 浮点数，`char` 字符， `bool` 布尔	

- 测试样例 (PrimitiveType.c)

  ```
  void main()
  {
    // 整型
    int x = 5;
    printf("%d ", x);
  
    // 浮点数型
    float f;
    f = 1.2;
    printf("%f ", f);
  
    // char型
    char c = '$';
    printf("%c ", c);
  
    // bool型
    bool y = true;
    printf("%d ", y);
  }
  ```

- 测试结果

  解释

  ![22](./README.assets/interpreter/22.png)

编译：

![](./README.assets/compile/20.png)

中间过程：

![ast](./README.assets/ast/20.png)

虚拟指令序列：

![seq](./README.assets/seq/20.png)

### 4.4.9 基本数学函数实现

- 实现`Max Min Abs` 等基本数学函数

- 测试样例 (ElementaryMathFunc.c)

  ```
  void main(int x, int y)
  {
    print Max(x, y);
    print Min(x, y);
    print Abs(x);
  }
  ```

- 测试结果

  解释

  ![23](./README.assets/interpreter/23.png)

编译：

![](./README.assets/compile/21.png)

中间过程：

![ast](./README.assets/ast/21.png)

虚拟指令序列：

![seq](./README.assets/seq/21.png)

### 4.4.10 try-catch异常捕获实现

- 实现/0的异常捕获

- 测试样例(Try_catch.c)

  ```
  int main()
  {
    try
    {
      int a = 0;
      int n = 5;
      n = n / a;
    }
    catch ("ArithmeticalExcption")
    {
      n = 0;
      print n;
    }
  }
  ```

- 测试结果

  编译：

  ![](./README.assets/compile/22.png)
  
  中间过程：
  
  ![ast](./README.assets/ast/22.png)
  
  虚拟指令序列：
  
  ![seq](./README.assets/seq/22.png)
  
  

### 4.10.11 强制类型转换

- int(x)、char(x)等基本强制类型转换功能的实现

- 测试样例(TypeCasting.c)

  ```
  void main(int x, char y)
  {
    printf("%d ", x);
    printf("%c ", (char)x);
    printf("%c ", y);
    printf("%d", (int)y);
  }
  ```

- 测试结果

  解释

  ![24](./README.assets/interpreter/24.png)

编译：

![](./README.assets/compile/23.png)

中间过程：

![ast](./README.assets/ast/23.png)

虚拟指令序列：

![seq](./README.assets/seq/23.png)

### 4.10.12 return静态作用域

- 测试样例(ReturnStatic.c)

  ```
  int x;
  int f(int y)
  {
    int x = y + 1;
    return x;
  }
  
  void main()
  {
    print f(3);
  }
  ```

- 测试结果

  解释

  ![25](./README.assets/interpreter/25.png)

编译：

![](./README.assets/compile/24.png)

中间过程：

![ast](./README.assets/ast/24.png)

虚拟指令序列：

![seq](./README.assets/seq/24.png)



# 5.课程心得

+ 许周毅：
  
  + 心得：本学期的课程任务繁重，而编译原理又是一门比较硬核的科目，在课程的学习上难免遇到了较多的困难，在课堂上未能很好的掌握，在课后还要投入大量的时间，同时还要学习F#这一个相对于C、JAVA来说比较冷门的语言，网上能够查到的参考学习资料也比较少，大作业的实现在起初更是可以说是压力山大、无从下手。好在还是有一些开源的项目能够供我们学习借鉴，虽然过程很艰辛，但还是在开源项目的帮助下，完成了大作业。而在课程的学习中，我了解了函数式编程，对计算机也有了更深入的理解。
  + 评价：课程的难度偏高，所需投入的时间较多；虽然给了许多参考资料，但有些抓不到重点。
  
+ 倪敏建：

  + 心得：拿到大作业要求的时候，心里是崩溃的。或许是对深挖底层原理的畏惧，从开学初就感觉编译原理这门课难度很大，在课程学习上遇到了很大的阻碍。一是因为这完全是一套全新的体系，与之前所学课程个人感觉没有很好的接轨，在这之前缺少了相关的知识储备；二是因为课程需要用到一门全新的语言F#，它作为一门函数式编程语言，与我们之前常用的C++、Java等面向对象的语言由很大的区别，而且资料非常少。但所幸老师给了许多资料，我们的大作业项目也是在研读这些资料的基础上一步步构建起来的。从自增、自减入门，一步步进行构建功能的完善；一遍遍地研读commits记录，抽取出实现过程。我们一个一个小功能的添加，及时调试，这个过程是十分痛苦的，但收获颇多。让我们对编译器、解释器有了更深刻的理解，逐渐与先前所学知识串联起来了。

  + 评价：这门课程的难度很高，知识点繁杂且串联度不高，这需要我们在课后花大量时间去理解、去记忆。同时，在真正着手大作业之前，缺少相关实验支撑。

    

# 6.附录

## 分工

| 姓名   | 学号     | 班级       | 任务                                     | 权重 |
| ------ | -------- | ---------- | ---------------------------------------- | ---- |
| 倪敏建 | 31904027 | 计算机1901 | 功能设计, 解释器，虚拟机，测试，报告撰写 | 0.95 |
| 许周毅 | 31903139 | 计算机1901 | 功能设计, 编译器，虚拟机，测试，报告撰写 | 0.95 |

## 日志

![](./README.assets/commit/1.png)

![](./README.assets/commit/2.png)

![](./README.assets/commit/3.png)



## 自评

实现：🟢

评分：1️⃣2️⃣3️⃣4️⃣5️⃣

|         功能          | **解释器** | **编译器** | 编译器优化 | 自评分（1-5） |        测试文件        |
| :-------------------: | :--------: | :--------: | :--------: | :-----------: | :--------------------: |
|     **词法功能**      |            |            |            |       5️⃣       |                        |
|     注释表示方式      |     🟢      |     🟢      |     🟢      |       5️⃣       |    commentaryTest.c    |
|      标识符定义       |     🟢      |     🟢      |     🟢      |       5️⃣       | IdentifierDefinition.c |
|       进制转换        |     🟢      |     🟢      |     🟢      |       5️⃣       |   RadixConversion.c    |
|     **语法功能**      |            |            |            |       5️⃣       |                        |
|        for循环        |     🟢      |     🟢      |     🟢      |       5️⃣       |      ForKeyWord.c      |
|     do-while循环      |     🟢      |     🟢      |     🟢      |       5️⃣       |    DoWhileKeyWord.c    |
|    switch-case判断    |     🟢      |     🟢      |     🟢      |       5️⃣       |  SwitchCaseKeyWord.c   |
|    break和continue    |            |     🟢      |     🟢      |       4️⃣       |   BreakAndContinue.c   |
|       三目运算        |     🟢      |     🟢      |     🟢      |       5️⃣       |   TernaryOperator.c    |
|        printf         |     🟢      |            |            |       3️⃣       |        printf.c        |
|       do-until        |     🟢      |     🟢      |     🟢      |       5️⃣       |       dountil.c        |
|        string         |     🟢      |            |            |       3️⃣       |      StringTest.c      |
|     **语义功能**      |            |            |            |               |                        |
|      静态作用域       |     🟢      |     🟢      |     🟢      |       5️⃣       |      StaticTest.c      |
|     **特性功能**      |            |            |            |               |                        |
|        位运算         |     🟢      |     🟢      |     🟢      |       5️⃣       |       Bitwise.c        |
|       逻辑运算        |     🟢      |     🟢      |     🟢      |       5️⃣       |   LogicalOperation.c   |
|        运算符         |     🟢      |     🟢      |     🟢      |       5️⃣       |       Operator.c       |
|    复合赋值运算符     |     🟢      |     🟢      |     🟢      |       5️⃣       |  CompoundAssignment.c  |
|      struct结构       |     🟢      |     🟢      |     🟢      |       5️⃣       |      StructTest.c      |
|         数组          |     🟢      |     🟢      |     🟢      |       5️⃣       |        Array.c         |
|      自定义函数       |     🟢      |     🟢      |     🟢      |       5️⃣       |  SelfDefinitionFunc.c  |
|     基本类型实现      |     🟢      |            |            |       3️⃣       |    PrimitiveType.c     |
|   基本数学函数实现    |     🟢      |     🟢      |     🟢      |       5️⃣       |  ElementaryMathFunc.c  |
| try-catch异常捕获实现 |            |     🟢      |     🟢      |       4️⃣       |      try_catch.c       |
|     强制类型转换      |     🟢      |            |            |       3️⃣       |     TypeCasting.c      |
|   return静态作用域    |     🟢      |     🟢      |     🟢      |       5️⃣       |     ReturnStatic.c     |
|                       |            |            |            |               |                        |





