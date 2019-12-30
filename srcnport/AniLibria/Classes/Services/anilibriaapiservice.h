#ifndef ANILIBRIAAPISERVICE_H
#define ANILIBRIAAPISERVICE_H

#include <QObject>
#include <QtNetwork>

class AnilibriaApiService : public QObject
{
    Q_OBJECT

public:
    explicit AnilibriaApiService(QObject *parent = nullptr);

    const static QString apiAddress;

    void getAllReleases();
    void getSchedule();
    void signin(QString email, QString password, QString fa2code);
    void signout(QString token);
    void getUserData(QString token);

signals:
    void allReleasesReceived(QString data);
    void scheduleReceived(QString data);
    void signinReceived(QString data);
    void signoutReceived();
    void userDataReceived(QString data);

public slots:
    void getAllReleasesResponse(QNetworkReply* reply);
    void getScheduleResponse(QNetworkReply* reply);
    void signinResponse(QNetworkReply* reply);
    void signoutResponse(QNetworkReply* reply);
    void getUserDataResponse(QNetworkReply* reply);
};

#endif // ANILIBRIAAPISERVICE_H
