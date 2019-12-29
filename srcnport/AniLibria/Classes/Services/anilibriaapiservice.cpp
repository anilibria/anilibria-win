#include "anilibriaapiservice.h"
#include <QtNetwork>
#include <QtDebug>

//const QString AnilibriaApiService::apiAddress = "https://anilibriasmartservice.azurewebsites.net/";
const QString AnilibriaApiService::apiAddress = "http://localhost:5001/";

AnilibriaApiService::AnilibriaApiService(QObject *parent) : QObject(parent)
{
}

void AnilibriaApiService::getAllReleases()
{
    auto networkManager = new QNetworkAccessManager(this);
    QNetworkRequest request(QUrl(AnilibriaApiService::apiAddress + "public/api/index.php"));
    request.setRawHeader("User-Agent", "Anilibria CP Client");
    request.setHeader(QNetworkRequest::ContentTypeHeader, QVariant("application/x-www-form-urlencoded"));

    QUrlQuery params;
    params.addQueryItem("query", "list");
    params.addQueryItem("page", "1");
    params.addQueryItem("perPage", "1000");

    connect(networkManager,SIGNAL(finished(QNetworkReply*)),this,SLOT(getAllReleasesResponse(QNetworkReply*)));

    networkManager->post(request, params.query(QUrl::FullyEncoded).toUtf8());
}

void AnilibriaApiService::getSchedule()
{
    auto networkManager = new QNetworkAccessManager(this);
    QNetworkRequest request(QUrl(AnilibriaApiService::apiAddress + "public/api/index.php"));
    request.setRawHeader("User-Agent", "Anilibria CP Client");
    request.setHeader(QNetworkRequest::ContentTypeHeader, QVariant("application/x-www-form-urlencoded"));
    QUrlQuery params;
    params.addQueryItem("query", "schedule");
    params.addQueryItem("filter", "id");

    connect(networkManager,SIGNAL(finished(QNetworkReply*)),this,SLOT(getScheduleResponse(QNetworkReply*)));

    networkManager->post(request, params.query(QUrl::FullyEncoded).toUtf8());
}

void AnilibriaApiService::signin(QString email, QString password, QString fa2code)
{
    auto networkManager = new QNetworkAccessManager(this);
    QNetworkRequest request(QUrl(AnilibriaApiService::apiAddress + "api/auth/signin?mail=" + email + "&password=" + password + "&fa2code=" + fa2code ));

    connect(networkManager,SIGNAL(finished(QNetworkReply*)),this,SLOT(signinResponse(QNetworkReply*)));

    networkManager->get(request);
}

void AnilibriaApiService::signout(QString token)
{
    auto networkManager = new QNetworkAccessManager(this);
    QNetworkRequest request(QUrl(AnilibriaApiService::apiAddress + "api/auth/signout?token=" + token ));

    connect(networkManager,SIGNAL(finished(QNetworkReply*)),this,SLOT(signoutResponse(QNetworkReply*)));

    networkManager->get(request);
}

void AnilibriaApiService::getAllReleasesResponse(QNetworkReply *reply)
{
    if (reply->error() == QNetworkReply::TimeoutError) return;
    if (reply->error() == QNetworkReply::ProtocolFailure) return;
    if (reply->error() == QNetworkReply::HostNotFoundError) return;

    QString data = reply->readAll();

    emit allReleasesReceived(data);
}

void AnilibriaApiService::getScheduleResponse(QNetworkReply *reply)
{
    if (reply->error() == QNetworkReply::TimeoutError) return;
    if (reply->error() == QNetworkReply::ProtocolFailure) return;
    if (reply->error() == QNetworkReply::HostNotFoundError) return;

    QString data = reply->readAll();

    emit scheduleReceived(data);
}

void AnilibriaApiService::signinResponse(QNetworkReply *reply)
{
    if (reply->error() == QNetworkReply::TimeoutError) return;
    if (reply->error() == QNetworkReply::ProtocolFailure) return;
    if (reply->error() == QNetworkReply::HostNotFoundError) return;

    emit signinReceived(reply->readAll());
}

void AnilibriaApiService::signoutResponse(QNetworkReply *reply)
{
    if (reply->error() == QNetworkReply::TimeoutError) return;
    if (reply->error() == QNetworkReply::ProtocolFailure) return;
    if (reply->error() == QNetworkReply::HostNotFoundError) return;

    emit signoutReceived();
}
