# Разработка расширений для автозавершения в Visual Studio 2019 и ReSharper
Из-за специфики моей работы, постоянно приходится иметь дело с новыми проектами. 
Уже давно в голове возникла мысль написать расширение, которое позволило бы видеть код утилит и расширений
из всех прошлых проектов в списке автозавершения с низким приоритетом и, после автозавершения, 
вставлять только нужные файлы в проект
(или вообще ограничиться только добавление зависимости на проект/библиотеку/nuget пакет). 
В данной статье я хочу познакомить вас с тем, что же вас ждет, если вы соберетесь писать 
расширение для актуальных Visual Studio или ReSharper в 2019
(используя AsyncPackage, IAsyncCompletionSource и прочие “новые” штуки)

## Возникшие проблемы
ReSharper plugin распространяется в виде nuget пакета с особой структорой
(все dll хранятся в подпапке /dotFiles). Мы можем включить нужные нам файлы, используя файл проекта(поддерживаются также WildCards - *). 
Если ваша библиотека использует зависимости, которых нет в SDK, не забудьте также включить их
(на самом деле, все сложнее и расписано здесь, но данный способ тоже работает) 
```
<Content Include="bin\$(Configuration)\OneCode.ReSharperExtension.*" PackagePath="dotFiles" Pack="true" />
```

## Ссылки по теме:
VSSDK-Extensibility-Samples - https://github.com/microsoft/VSSDK-Extensibility-Samples/
Пишем простейший плагин для ReSharper - https://habr.com/ru/post/270155/
Официальное руководство ReSharper - ReSharper DevGuide - https://www.jetbrains.com/resharper/devguide/README.html
Google группа Reshaper - resharper-plugins - https://groups.google.com/forum/#!forum/resharper-plugins
JetBrains Developer Community - https://devnet.jetbrains.com/community/resharper/resharper_open_api?view=discussions