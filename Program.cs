try {
    // Проверка аргументов
    if (args.Length == 0) throw new ArgumentNullException("Not found arguments!");
    if (args[0] == "-help") {
        ShowHelp(); return;
    }

    // Входные данные
    string json = File.ReadAllText(args[1]);
    Data data = JsonConvert.DeserializeObject<Data>(json)!;
    if (data is null) throw new FileNotFoundException("File uncorrected!");

    // Проверка входных данных
    if (!data.Incorrect(out string mes)) throw new ArgumentException(mes);

    // Определение функции
    Function.Init(data.N);

    // Генерация сетки
    Generate generator = new Generate(data, Path.GetDirectoryName(args[1])!);
    generator.SetKraev(1, 1, 1, 1);
    Grid grid = generator.generate();

    // Метод МКЭ
    FEM task = new FEM(grid, Path.GetDirectoryName(args[1])!);
    task.solve();
}
catch (FileNotFoundException ex) {
    WriteLine(ex.Message);
}
catch (ArgumentNullException ex) {
    ShowHelp();
    WriteLine(ex.Message);
}
catch (ArgumentException ex) {
    WriteLine(ex.Message);
}
