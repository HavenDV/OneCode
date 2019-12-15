# ���������� ���������� ��� �������������� � Visual Studio 2019 � ReSharper
��-�� ��������� ���� ������, ��������� ���������� ����� ���� � ������ ���������. 
��� ����� � ������ �������� ����� �������� ����������, ������� ��������� �� ������ ��� ������ � ����������
�� ���� ������� �������� � ������ �������������� � ������ ����������� �, ����� ��������������, 
��������� ������ ������ ����� � ������
(��� ������ ������������ ������ ���������� ����������� �� ������/����������/nuget �����). 
� ������ ������ � ���� ����������� ��� � ���, ��� �� ��� ����, ���� �� ���������� ������ 
���������� ��� ���������� Visual Studio ��� ReSharper � 2019
(��������� AsyncPackage, IAsyncCompletionSource � ������ ������ �����)

## ��������� ��������
ReSharper plugin ���������������� � ���� nuget ������ � ������ ����������
(��� dll �������� � �������� /dotFiles). �� ����� �������� ������ ��� �����, ��������� ���� �������(�������������� ����� WildCards - *). 
���� ���� ���������� ���������� �����������, ������� ��� � SDK, �� �������� ����� �������� ��
(�� ����� ����, ��� ������� � ��������� �����, �� ������ ������ ���� ��������) 
```
<Content Include="bin\$(Configuration)\OneCode.ReSharperExtension.*" PackagePath="dotFiles" Pack="true" />
```

## ������ �� ����:
VSSDK-Extensibility-Samples - https://github.com/microsoft/VSSDK-Extensibility-Samples/
����� ���������� ������ ��� ReSharper - https://habr.com/ru/post/270155/
����������� ����������� ReSharper - ReSharper DevGuide - https://www.jetbrains.com/resharper/devguide/README.html
Google ������ Reshaper - resharper-plugins - https://groups.google.com/forum/#!forum/resharper-plugins
JetBrains Developer Community - https://devnet.jetbrains.com/community/resharper/resharper_open_api?view=discussions