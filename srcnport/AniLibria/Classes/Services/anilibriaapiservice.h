#ifndef ANILIBRIAAPISERVICE_H
#define ANILIBRIAAPISERVICE_H

#include <QObject>
#include <QtNetwork>

class AnilibriaApiService : public QObject
{
    Q_OBJECT

public:
    explicit AnilibriaApiService(QObject *parent = nullptr);

    void getAllReleases();
    void getSchedule();

signals:
    void allReleasesReceived(QString data);
    void scheduleReceived(QString data);

public slots:
    void getAllReleasesResponse(QNetworkReply* reply);
    void getScheduleResponse(QNetworkReply* reply);
};

#endif // ANILIBRIAAPISERVICE_H
