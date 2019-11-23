#ifndef LOCALSTORAGESERVICE_H
#define LOCALSTORAGESERVICE_H

#include <QObject>
#include <QSqlDatabase>

class LocalStorageService : public QObject
{
    Q_OBJECT

private:
    QSqlDatabase m_Database;

public:
    explicit LocalStorageService(QObject *parent = nullptr);

    ~LocalStorageService();

signals:

public slots:
};

#endif // LOCALSTORAGESERVICE_H
