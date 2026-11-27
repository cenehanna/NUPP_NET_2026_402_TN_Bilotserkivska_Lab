#include "CryptoRGR.h"
#include <QPushButton>
#include <QLineEdit>
#include <QFileDialog>
#include <QMessageBox>
#include <fstream>
#include <vector>

CryptoRGR::CryptoRGR(QWidget* parent)
    : QMainWindow(parent)
{
    ui.setupUi(this);

    // Зв'язуємо клік кнопки з функцією шифрування
    connect(ui.pushButtonEncrypt, &QPushButton::clicked, this, &CryptoRGR::onEncryptClicked);
    connect(ui.pushButtonDecrypt, &QPushButton::clicked, this, &CryptoRGR::onDecryptClicked);
}

CryptoRGR::~CryptoRGR()
{
}

void CryptoRGR::onEncryptClicked() {
    // 1. Отримуємо головний ключ із текстового поля lineEditKey
    QString key = ui.lineEditKey->text();
    if (key.isEmpty()) {
        QMessageBox::warning(this, "Помилка", "Будь ласка, введіть головний ключ!");
        return;
    }

    // 2. Відкриваємо діалог для вибору вхідного файлу довільної структури (.txt, .exe тощо)
    QString filePath = QFileDialog::getOpenFileName(this, "Обрати файл для шифрування");
    if (filePath.isEmpty()) return;

    // 3. Зчитування файлу у двійковому режимі
    std::ifstream inFile(filePath.toStdString(), std::ios::binary);
    if (!inFile) {
        QMessageBox::critical(this, "Помилка", "Не вдалося відкрити вхідний файл!");
        return;
    }

    // Читаємо весь файл у вектори байтів (unsigned char)
    std::vector<unsigned char> buffer(
        (std::istreambuf_iterator<char>(inFile)),
        std::istreambuf_iterator<char>()
    );
    inFile.close();

    if (buffer.empty()) {
        QMessageBox::warning(this, "Попередження", "Обраний файл порожній!");
        return;
    }

    // 4. ЗАСТОСУВАННЯ КОМБІНОВАНИХ МЕТОДІВ ШИФРУВАННЯ
    // Метод 1: Проста підстановка (зсув байта) + Метод 2: Гаммування (XOR)
    int shift = 0;
    for (QChar c : key) {
        shift += c.unicode();
    }
    shift %= 256;

    for (size_t i = 0; i < buffer.size(); ++i) {
        // Застосовуємо перший метод
        buffer[i] = static_cast<unsigned char>((buffer[i] + shift) % 256);

        // Застосовуємо другий метод
        unsigned char gammaByte = static_cast<unsigned char>(key[i % key.length()].unicode());
        buffer[i] ^= gammaByte;
    }

    // 5. Обираємо місце та ім'я для збереження зашифрованого файлу
    QString outPath = QFileDialog::getSaveFileName(this, "Зберегти зашифрований файл як", "", "Binary Files (*.bin);;All Files (*.*)");
    if (outPath.isEmpty()) return;

    // Записуємо змінені байти у вихідний файл
    std::ofstream outFile(outPath.toStdString(), std::ios::binary);
    if (!outFile) {
        QMessageBox::critical(this, "Помилка", "Не вдалося створити вихідний файл!");
        return;
    }

    outFile.write(reinterpret_cast<const char*>(buffer.data()), buffer.size());
    outFile.close();

    QMessageBox::information(this, "Успіх", "Файл успішно зашифровано комбінованим методом!");
}
void CryptoRGR::onDecryptClicked() {
    // 1. Отримуємо головний ключ
    QString key = ui.lineEditKey->text();
    if (key.isEmpty()) {
        QMessageBox::warning(this, "Помилка", "Введіть головний ключ для розшифрування!");
        return;
    }

    // 2. Обираємо зашифрований файл
    QString filePath = QFileDialog::getOpenFileName(this, "Обрати зашифрований файл");
    if (filePath.isEmpty()) return;

    // 3. Читаємо файл у двійковому режимі
    std::ifstream inFile(filePath.toStdString(), std::ios::binary);
    if (!inFile) {
        QMessageBox::critical(this, "Помилка", "Не вдалося відкрити файл!");
        return;
    }

    std::vector<unsigned char> buffer(
        (std::istreambuf_iterator<char>(inFile)),
        std::istreambuf_iterator<char>()
    );
    inFile.close();

    // 4. Зворотне перетворення (у зворотному порядку!)
    int shift = 0;
    for (QChar c : key) {
        shift += c.unicode();
    }
    shift %= 256;

    for (size_t i = 0; i < buffer.size(); ++i) {
        // Спочатку скасовуємо другий метод (гаммування XOR) — воно симетричне
        unsigned char gammaByte = static_cast<unsigned char>(key[i % key.length()].unicode());
        buffer[i] ^= gammaByte;

        // Потім скасовуємо перший метод (віднімаємо зсув)
        buffer[i] = static_cast<unsigned char>((buffer[i] - shift + 256) % 256);
    }

    // 5. Зберігаємо розшифрований файл
    QString outPath = QFileDialog::getSaveFileName(this, "Зберегти розшифрований файл як", "", "All Files (*.*)");
    if (outPath.isEmpty()) return;

    std::ofstream outFile(outPath.toStdString(), std::ios::binary);
    if (!outFile) {
        QMessageBox::critical(this, "Помилка", "Не вдалося зберегти файл!");
        return;
    }

    outFile.write(reinterpret_cast<const char*>(buffer.data()), buffer.size());
    outFile.close();

    QMessageBox::information(this, "Успіх", "Файл успішно розшифровано!");
}