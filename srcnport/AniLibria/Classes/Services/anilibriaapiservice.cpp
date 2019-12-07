#include "anilibriaapiservice.h"
#include <QtNetwork>
#include <QtDebug>

AnilibriaApiService::AnilibriaApiService(QObject *parent) :
    QObject(parent),
    m_NetworkManager()
{
}

void AnilibriaApiService::getAllReleases()
{
    auto networkManager = new QNetworkAccessManager(this);

    QNetworkRequest request(QUrl("https://anilibriasmartservice.azurewebsites.net/public/api/index.php"));
    request.setRawHeader("User-Agent", "Anilibria CP Client");
    request.setHeader(QNetworkRequest::ContentTypeHeader,QVariant("application/x-www-form-urlencoded"));

    QUrlQuery params;
    params.addQueryItem("query", "list");
    params.addQueryItem("page", "1");
    params.addQueryItem("perPage", "1000");

    connect(networkManager,SIGNAL(finished(QNetworkReply*)),this,SLOT(getAllReleasesResponse(QNetworkReply*)));

    networkManager->post(request,params.query(QUrl::FullyEncoded).toUtf8());
}

void AnilibriaApiService::getSchedule()
{
    QNetworkRequest request(QUrl("https://www.anilibria.tv/public/api/index.php"));
    request.setRawHeader("User-Agent", "Anilibria CP Client");
    request.setHeader(QNetworkRequest::ContentTypeHeader,QVariant("application/x-www-form-urlencoded"));
    QUrlQuery params;
    params.addQueryItem("query", "schedule");
    params.addQueryItem("filter", "id");

    connect(
        m_NetworkManager,
        &QNetworkAccessManager::finished,
        this,
        [=](QNetworkReply *reply) {
            if (reply->error()) {
                qDebug() << reply->errorString();
                return;
            }

            QString answer = reply->readAll();

            emit scheduleReceived(answer);
        }
    );

}

void AnilibriaApiService::getAllReleasesResponse(QNetworkReply *reply)
{
    if (reply->error() == QNetworkReply::TimeoutError) return;
    if (reply->error() == QNetworkReply::ProtocolFailure) return;
    if (reply->error() == QNetworkReply::HostNotFoundError) return;

    QString data = reply->readAll();

    emit allReleasesReceived(data);
}
