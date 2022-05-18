(* File MicroC/Absyn.fs
   Abstract syntax of micro-C, an imperative language.
   sestoft@itu.dk 2009-09-25

   Must precede Interp.fs, Comp.fs and Contcomp.fs in Solution Explorer
 *)

// 定义了变量描述、函数和类型的构造方法
module Absyn

// 基本类型
// 注意，数组、指针是递归类型
// 这里没有函数类型，注意与上次课的 MicroML 对比
// 这里使用的是显式类型函数式语言，所有变量都需要写出定义
// typ 是 某个`绑定值`在运行时可以具备的类型
type typ =
  | TypI                             (* Type int                    *)
  | TypC                             (* Type char                   *)
  | TypA of typ * int option         (* Array type，注意定义数组类型时要指明存储的变量类型*)
  | TypP of typ                      (* Pointer type                *)
  | TypS
  | TypF
  | TypB
  | TypStruct of string


(*Micro实现解释器*)
(*各种构造器的构建*)
(*
  注意这里对左值表达式与右值表达式的构建，右值是常规值，左值是数字
  大量的运算符都是右值表达式，左值表达式中包含了一些特殊操作，我们在这主要定义了变量访问符、指针引用符、数组下标符
*)

and expr =                           // 右值表达式                                            
  | Access of access                 (* x    or  *p    or  a[e]     *) //访问左值（右值）
  | Assign of access * expr          (* x=e  or  *p=e  or  a[e]=e   *)

  // 基本运算表达式的定义，加、减、乘、除、取余
  // | PlusAssign of access * expr      (* x+=e or  *p+=e or  a[e]+=e  *)
  // | MinusAssign of access * expr     (* x-=e or  *p-=e or  a[e]-=e  *)
  // | TimesAssign of access * expr     (* x*=e or  *p*=e or  a[e]*=e  *)
  // | DivAssign of access * expr       (* x/=e or  *p/=e or  a[e]/=e  *)
  // | ModAssign of access * expr       (* x%=e or  *p%=e or  a[e]%=e  *)
  | OpAssign of string * access * expr
  | Addr of access                   (* &x   or  &*p   or  &a[e]    *)
  | CstI of int                      (* Constant                    *)
  | CstC of char                     (* char类型  *)
  | CstS of string
  | CstF of float32
  // | CstNull
  | Prim1 of string * expr           (* Unary primitive operator    *)
  | Prim2 of string * expr * expr    (* Binary primitive operator   *)
  | Prim3 of expr * expr * expr      (* 三目运算 e1 ? e2 : e3 *)
  | Printf of string * expr list     (* 格式化输出 *)
  | Andalso of expr * expr           (* Sequential and              *)
  | Orelse of expr * expr            (* Sequential or               *)
  | Call of string * expr list       (* Function call f(...)        *)
  | PreInc of access                 (* 自增 ++x or ++a[i]*)
  | PreDec of access                 (* 自减--x or --a[i]*)
  | NextInc of access                  (*x++ or a[i]--*)
  | NextDec of access                  (*x-- or a[i]--*)
  | ToChar of expr
  | ToInt of expr
  | Max of expr * expr               (* Max function                *)
  | Min of expr * expr               (* Min function                *)
  | Abs of expr                      (* Abs function                *)

and access =                         //左值，存储的位置                                            
  | AccVar of string                 (* Variable access        x    *) 
  | AccDeref of expr                 (* Pointer dereferencing  *p   *)
  | AccIndex of access * expr        (* Array indexing         a[e] *)
  | AccStruct of access * access

and stmt =                                                         
  | If of expr * stmt * stmt         (* Conditional                 *)
  | Switch of expr * caseStmt list       (* Switch case语句 case有多个 因此是个list*)
  // | Case of expr * stmt              (* Case  需要条件和要执行的语句*)
  // | Default of stmt                  (* Switch case 缺省语句 *)
  | While of expr * stmt             (* While loop                  *)
  | For of expr * expr * expr * stmt (* For循环 *)
  | DoWhile of stmt * expr           (* dowhile 循环*)
  | DoUntil of stmt * expr
  | Break
  | Continue
  | Expr of expr                     (* Expression statement   e;   *)
  | Return of expr option            (* Return from method          *)
  | Block of stmtordec list          (* Block: grouping and scope   *)
  | Throw of excep
  | Try of stmt * stmt list
  | Catch of excep * stmt
  // 语句块内部，可以是变量声明 或语句的列表                                                              

and caseStmt =  // switch case中用到的type
  | Case of expr * stmt
  | Default of stmt

and excep = 
  | Exception of string

and stmtordec =                                                    
  | Dec of typ * string              (* Local variable declaration  *)
  | DecAndAssign of typ * string * expr
  | Stmt of stmt                     (* A statement                 *)

// 顶级声明 可以是函数声明或变量声明
and topdec = 
  | Fundec of typ option * string * (typ * string) list * stmt
  | Vardec of typ * string
  | Structdec of string * (typ * string) list
  | VardecAndAssign of typ * string * expr

// 程序是顶级声明的列表
and program = 
  | Prog of topdec list
