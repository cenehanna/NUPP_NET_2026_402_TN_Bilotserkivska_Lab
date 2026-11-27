#include "CryptoRGR.h"
#include <QtWidgets/QApplication>

int main(int argc, char *argv[])
{
    QApplication app(argc, argv);
    CryptoRGR window;
    window.show();
    return app.exec();
}
