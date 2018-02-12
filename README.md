# calculator
Claculator, MVVC architecture. Was developed with using .NET Core.

This app uses my CalcOperator package that is publicly available at
https://www.nuget.org/packages/CalcOperator/

Be sure, that this package is included in the app's references.


<p>
    <br />
        This app was made using .NET Core (MVVC) tools.
    <br /><br />
        User can upload and remove operators (*.dll files) dynamically. 
        The <a href="./Example">operators</a> must extend either abstract class <a href="./Abstract">IGeneralOperator</a> or <a href="./Abstract">
    IOperator</a>.
    <br /><br />
    For converting infix notation to postfix (Reverse Polish Notation),
    <strong>Shunting-yard algorithm</strong> was used.
    <br /><br />
    For calculation,
    <strong>Reverse Polish Notation algorithm</strong> was used.
    <br /><br />
    Client-server communication is implemented with AJAX calls.
    <br /><br />
</p>
