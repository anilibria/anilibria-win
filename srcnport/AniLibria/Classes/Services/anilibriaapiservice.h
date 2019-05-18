#ifndef ANILIBRIAAPISERVICE_H
#define ANILIBRIAAPISERVICE_H

#include <QObject>
#include <QtNetwork>

class AnilibriaApiService : public QObject
{
    Q_OBJECT
private:

public:
    explicit AnilibriaApiService(QObject *parent = nullptr);

    void getAllReleases();

signals:
    void allReleasesReceived(QString data);

public slots:
    void getAllReleasesResponse(QNetworkReply* reply);
};

#endif // ANILIBRIAAPISERVICE_H
