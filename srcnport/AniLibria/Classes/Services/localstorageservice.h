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

    Q_INVOKABLE void AddRelease(const QString& release);
    Q_INVOKABLE void UpdateRelease(const QString& release);
    Q_INVOKABLE QString GetRelease(int id);
    Q_INVOKABLE QStringList GetReleasesPage(int page);
signals:

public slots:


};

#endif // LOCALSTORAGESERVICE_H
