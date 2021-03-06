### Предисловие
Работающее приложение - как машина. Классы - как детали. Чем качественней класс - тем дольше "машина" проедет до следующего "ремонта".
Чтобы детали были качественнее, желательно их переиспользовать в разных средах - только так можно отловить различные проблемы и улучшить их.
Да, вы все правильно поняли - я перфекционист. Хорошо это или плохо - отдельный разговор.
Но именно с этими мыслями я приступил к разработке расширения "для себя", которое позволяло бы хранить эти универсальные детальки, оттачивать их и использовать в любом проекте в любое время.
Попутно написав статью о текущих реалиях разработки расширений для IDE.

# Разработка расширений для автозавершения в Visual Studio 2019 и ReSharper

Из-за специфики моей работы, постоянно приходится иметь дело с новыми проектами. 
Уже давно в голове возникла мысль написать расширение, которое позволило бы видеть код утилит и расширений
из всех прошлых проектов в списке автозавершения с низким приоритетом и, после автозавершения, 
вставлять только нужные файлы в проект
(или вообще ограничиться только добавление зависимости на проект/библиотеку/nuget пакет). 
В данной статье я хочу познакомить вас с тем, что же вас ждет, если вы соберетесь писать 
расширение для актуальных Visual Studio или ReSharper в 2020
(используя `AsyncPackage`, `IAsyncCompletionSource` и прочие “новые” штуки)

## Возникшие проблемы
ReSharper plugin распространяется в виде nuget пакета с особой структорой
(все dll хранятся в подпапке /dotFiles). Мы можем включить нужные нам файлы, используя файл проекта(поддерживаются также WildCards - *). 
Если ваша библиотека использует зависимости, которых нет в SDK, не забудьте также включить их
(на самом деле, все сложнее и расписано здесь, но данный способ тоже работает) 
```
<Content Include="bin\$(Configuration)\YourFilesPrefix.*" PackagePath="dotFiles" Pack="true" />
```

## Ссылки по теме:
VSSDK-Extensibility-Samples - https://github.com/microsoft/VSSDK-Extensibility-Samples/ </br>
Пишем простейший плагин для ReSharper - https://habr.com/ru/post/270155/ </br>
ReSharper DevGuide - https://www.jetbrains.com/resharper/devguide/README.html </br>
Google группа Reshaper - resharper-plugins - https://groups.google.com/forum/#!forum/resharper-plugins </br>
JetBrains Developer Community - https://devnet.jetbrains.com/community/resharper/resharper_open_api?view=discussions </br>
